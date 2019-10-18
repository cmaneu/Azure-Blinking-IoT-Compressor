using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Samples.IoTCompressor.Backend
{
    public static class CreateDevice
    {
        static readonly string connectionString = Environment.GetEnvironmentVariable("iotHubConnectionString");
        static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        
        [FunctionName("CreateDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)] HttpRequest req,
            ILogger log)
        {
            string deviceName = req.Query["deviceName"];
            string deviceNumber = req.Query["deviceNumber"];
            log.LogInformation($"DeviceCreation Request for {deviceName} #{deviceNumber}");

            var device = new Device(deviceName);
            var creationTwin = new Twin
                {
                    Tags = new TwinCollection(@"{ deviceName: '" + deviceName + "' }"),
                    Properties = new TwinProperties() {
                        Desired = new TwinCollection(@"{ deviceNumber: '" + deviceNumber + "', led: {r: 100, g: 50, b: 0 }}")
                    }
                };
            
                        var registryOperation = await registryManager.AddDeviceWithTwinAsync(device, creationTwin);
            var createdDevice = await registryManager.GetDeviceAsync(deviceName);
            
            
            return (ActionResult)new OkObjectResult($"HostName=iotworkshopfr.azure-devices.net;DeviceId={deviceName};SharedAccessKey={createdDevice.Authentication.SymmetricKey.PrimaryKey}");
        }
    }
}
