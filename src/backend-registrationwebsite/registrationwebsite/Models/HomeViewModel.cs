using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace registrationwebsite.Models
{
    public class HomeViewModel
    {
        private List<SelectListItem> _deviceIds;

        private IEnumerable<DeviceCallback> _deviceCallbacks;

        public string DeviceId{get; set;}

        public string AzureFunctionUrl{get; set;}
        
        public HomeViewModel(IEnumerable<string> deviceIds, IEnumerable<DeviceCallback> deviceCallbacks)
        {
            _deviceIds = new List<SelectListItem>();
            foreach(string deviceId in deviceIds)
            {
                _deviceIds.Add(new SelectListItem(){Value=deviceId, Text=deviceId});
            }

            _deviceCallbacks = deviceCallbacks;
        }

        public List<SelectListItem> DeviceIds{
            get
            {
                return _deviceIds;
            }
        }

        public IEnumerable<DeviceCallback>  DeviceCallbacks{
            get
            {
                return _deviceCallbacks;
            }
        }
    }
}