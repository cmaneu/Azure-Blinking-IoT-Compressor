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

        public async Task<IActionResult> Index()
        {
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

            TableQuery query = new TableQuery()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "devfestnantes19" )
                    );

            IEnumerable<DeviceCallback> deviceCallbacks = _deviceMappingTable
                .ExecuteQuery(query)
                .Select(x => new DeviceCallback("devfestnantes19", x.RowKey, x.Properties.ContainsKey("callbackUrl") ? x.Properties["callbackUrl"].StringValue : ""));
            

            return View(new HomeViewModel(deviceIdList, deviceCallbacks));
        }

        [HttpPost]
        public IActionResult Register(string DeviceId, string AzureFunctionUrl)
        {
            _logger.LogInformation("Device Id: {0}, Callback Url: {1}", DeviceId, AzureFunctionUrl);

            DeviceCallback deviceCallback = new DeviceCallback("devfestnantes19", DeviceId, AzureFunctionUrl);
            TableOperation operation = TableOperation.InsertOrReplace(deviceCallback);

            TableResult result = _deviceMappingTable.Execute(operation);
            _logger.LogInformation("HttpStatusCode {0}", result.HttpStatusCode);

            return Redirect("/");
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
