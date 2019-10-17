using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace registrationwebsite
{
    public class HomeViewModel
    {
        private List<SelectListItem> _deviceIds;

        public string DeviceId{get; set;}

        public string AzureFunctionUrl{get; set;}
        
        public HomeViewModel(IEnumerable<string> deviceIds)
        {
            _deviceIds = new List<SelectListItem>();
            foreach(string deviceId in deviceIds)
            {
                _deviceIds.Add(new SelectListItem(){Value=deviceId, Text=deviceId});
            }
            Console.WriteLine(_deviceIds.Count);
        }

        public List<SelectListItem> DeviceIds{
            get
            {
                Console.WriteLine(_deviceIds.Count);
                return _deviceIds;
            }
        }
    }
}