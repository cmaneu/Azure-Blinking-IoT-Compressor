using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace Microsoft.Samples.IoTCompressor.Backend
{
    public static class OnHubMessageReceived
    {
        static readonly string connectionString = Environment.GetEnvironmentVariable("iotHubConnectionString");
        static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("OnHubMessageReceived")]
        public static async Task Run([EventHubTrigger("iot-events", Connection = "iotworkshop-dev_iothubroutes_iotworkshopfr_EVENTHUB")] EventData[] events, ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var storageAccount = CreateStorageAccountFromConnectionString(config["RegistryStorage"]);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var deviceMappingTable = tableClient.GetTableReference(DeviceMapping.TABLE_NAME);

            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    if (eventData.Properties.ContainsKey("deviceName"))
                    {
                        string deviceName = eventData.Properties["deviceName"].ToString();
                        log.LogInformation($"New message from device {deviceName}");
                        var mapping = await GetDeviceMapping(deviceMappingTable, "devfestnantes19", deviceName);
                        log.LogInformation($"Mapping found for device {deviceName}: {mapping.Callbackurl}");
                        
                        var deviceEventMessage = JsonConvert.DeserializeObject<EventHubMessage>(messageBody) as EventHubMessage;

                        Uri callbackUri;
                        if (Uri.TryCreate(mapping.Callbackurl, UriKind.Absolute, out callbackUri))
                        {

                            var requestContent = new StringContent(JsonConvert.SerializeObject(new WebHookRequestMessage()
                            {
                                deviceId = deviceName,
                                temperature = deviceEventMessage.T,
                                humidity = deviceEventMessage.H,
                                pressure = deviceEventMessage.P
                            }));
                            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                            var request = await _httpClient.PostAsync(callbackUri, requestContent);
                            if (!request.IsSuccessStatusCode)
                            {
                                log.LogInformation($"Webhook call failed for device {deviceName}. Status Code: {request.StatusCode}");
                            }
                            else
                            {
                                // TODO : parse this and send an update to the IoT Hub Device Twin
                                string content = await request.Content.ReadAsStringAsync();
                                var response = JsonConvert.DeserializeObject<WebHookResponseMessage>(content);
                                await SetDeviceTwin(deviceName, response.led.r, response.led.g, response.led.b);
                                log.LogInformation($"DeviceTwin for {deviceName} set to {response.led.r},{response.led.g},{response.led.b}.");
                            }
                        }                       
                    }
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    log.LogError(e, "Unable to parse response from evaluation webhook.");
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        public static async Task<DeviceMapping> GetDeviceMapping(CloudTable table, string eventName, string deviceName)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<DeviceMapping>(eventName, deviceName);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                DeviceMapping mapping = result.Result as DeviceMapping;
                return mapping;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        public static async Task SetDeviceTwin(string deviceName, int ledR, int ledG, int ledB)
        {
              var twin = await registryManager.GetTwinAsync(deviceName);
                 var patch = new
                {
                    properties = new
                    {
                        desired = new
                        {
                            led = new 
                            {
                                r = ledR,
                                g = ledG,
                                b = ledB
                            }
                        }
                    }
                };
            await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
        }
    }


    public class DeviceMapping : TableEntity
    {
        public static string TABLE_NAME = "webhooks";
        public DeviceMapping()
        {
        }

        public DeviceMapping(string eventName, string deviceName)
        {
            PartitionKey = eventName;
            RowKey = deviceName;
        }

        public string Callbackurl { get; set; }

        public string DeviceName 
        { 
            get => RowKey;
            set => RowKey = value;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            this.Callbackurl = properties["callbackUrl"].StringValue;
        }
    }


    public class EventHubMessage
    {
        public string Topic { get; set; }
        public string UpdateId { get; set; }
        public float T { get; set; }
        public float P { get; set; }
        public float H { get; set; }
    }

    public class WebHookRequestMessage
    {
        public string deviceId { get; set; }
        public float temperature { get; set; }
        public float pressure { get; set; }
        public float humidity { get; set; }
    }

    public class WebHookResponseMessage
    {
        public string state { get; set; }
        
        public WebHookResponseLed led {get; set;}

        public class WebHookResponseLed 
        {
            public int r {get;set;}
            public int g {get;set;}
            public int b {get;set;}
        }
    }
}
