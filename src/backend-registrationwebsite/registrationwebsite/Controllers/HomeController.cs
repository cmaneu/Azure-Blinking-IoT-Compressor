using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using registrationwebsite.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Cosmos.Table;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace registrationwebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegistryManager _registryManager;

        private readonly CloudTable _deviceMappingTable;

        public HomeController(ILogger<HomeController> logger, RegistryManager registryManager, CloudTable deviceMappingTable)
        {
            _logger = logger;
            _registryManager = registryManager;
            _deviceMappingTable = deviceMappingTable;
        }

        public async Task<IActionResult> Index(string DeviceId, string CallbackUrl, string InformationMessage)
        {
            _logger.LogInformation("DeviceId: {0}, CallbackUrl: {1}, InformationMessage: {2}", DeviceId, CallbackUrl, InformationMessage);    

            IQuery deviceIdQuery = _registryManager.CreateQuery("select * from devices");
            List<string> deviceIdList = new List<string>();
            while(deviceIdQuery.HasMoreResults)
            {
                IEnumerable<Twin> twins = await deviceIdQuery.GetNextAsTwinAsync().ConfigureAwait(false);
                foreach(Twin twin in twins)
                {
                    deviceIdList.Add(twin.DeviceId);
                }
            }

            deviceIdList.Sort();
            
            TableQuery query = new TableQuery()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "devfestnantes19" )
                    );

            IEnumerable<DeviceCallback> deviceCallbacks = _deviceMappingTable
                .ExecuteQuery(query)
                .Select(x => new DeviceCallback("devfestnantes19", x.RowKey, x.Properties.ContainsKey("callbackUrl") ? x.Properties["callbackUrl"].StringValue : ""));
            
            HomeViewModel model = new HomeViewModel(deviceIdList, deviceCallbacks);
            model.DeviceId = DeviceId;
            model.CallbackUrl = CallbackUrl;
            model.InformationMessage = InformationMessage; 
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(string DeviceId, string CallbackUrl)
        {
            _logger.LogInformation("DeviceId: {0}, CallbackUrl: {1}", DeviceId, CallbackUrl);

            DeviceCallback deviceCallback = new DeviceCallback("devfestnantes19", DeviceId, CallbackUrl);
            TableOperation operation = TableOperation.InsertOrReplace(deviceCallback);

            TableResult result = _deviceMappingTable.Execute(operation);
            _logger.LogInformation("HttpStatusCode {0}", result.HttpStatusCode);

            return Redirect(
                String.Format(
                    "/?DeviceId={0}&CallbackUrl={1}&InformationMessage={2}", 
                    HttpUtility.UrlEncode(DeviceId), 
                    HttpUtility.UrlEncode(CallbackUrl),
                    HttpUtility.UrlEncode("Your callback url has been successfully registered")));
        }

        [HttpPost]
        public async Task<IActionResult> Test(string DeviceId, string CallbackUrl)
        {
            _logger.LogInformation("DeviceId: {0}, CallbackUrl: {1}", DeviceId, CallbackUrl);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(CallbackUrl, 
                    new StringContent(JsonSerializer.Serialize(
                        new {
                            deviceId=DeviceId, 
                            temperature=23.4, 
                            pressure=1004.9,
                            humidity=52.7
                            }))
                );

            string responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("StatusCode: {0}, Body: {1}", response.StatusCode, responseContent);
            
            string informationMessage;
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                informationMessage = String.Format("The test is successful, the function response is {0}", responseContent);
            }
            else
            {
                informationMessage = "The test has failed";
            }

            return Redirect(
                String.Format(
                    "/?DeviceId={0}&CallbackUrl={1}&InformationMessage={2}", 
                    HttpUtility.UrlEncode(DeviceId), 
                    HttpUtility.UrlEncode(CallbackUrl),
                    HttpUtility.UrlEncode(informationMessage)));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
