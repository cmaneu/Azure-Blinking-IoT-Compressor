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

namespace registrationwebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegistryManager _registryManager;

        public HomeController(ILogger<HomeController> logger, RegistryManager registryManager)
        {
            _logger = logger;
            _registryManager = registryManager;
        }

        public async Task<IActionResult> Index()
        {
            IQuery deviceIdQuery = _registryManager.CreateQuery("select * from devices");
            List<string> deviceIdList = new List<string>();
            while(deviceIdQuery.HasMoreResults)
            {
                IEnumerable<Twin> twins = await deviceIdQuery.GetNextAsTwinAsync().ConfigureAwait(false);
                _logger.LogInformation("{0} twins", twins.Count().ToString());
                foreach(Twin twin in twins)
                {
                    _logger.LogInformation("DeviceId: {0}", twin.DeviceId);
                    deviceIdList.Add(twin.DeviceId);
                }
            }

            return View(new HomeViewModel(deviceIdList));
        }

        [HttpPost]
        public IActionResult Register([Bind("DeviceId")] string deviceId)
        {
            _logger.LogInformation("Device Id: {0}", deviceId);
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
