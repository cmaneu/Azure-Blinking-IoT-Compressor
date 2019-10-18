using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace registrationwebsite.Models
{
    public class DeviceCallback : TableEntity
    {
        public DeviceCallback(string eventName, string deviceId, string callbackUrl)
        {
            this.PartitionKey = eventName;
            this.RowKey = deviceId;
            this.CallbackUrl = callbackUrl;
        }

        public string EventName
        {
            get => this.PartitionKey;
        }

        public string DeviceId 
        {
            get => this.RowKey;
        }

        public string CallbackUrl
        {
            get;
            set;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            this.CallbackUrl = properties["callbackUrl"].StringValue;
        }

        public override  IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext){
            Dictionary<string, EntityProperty> properties = new Dictionary<string, EntityProperty>();
            properties["callbackUrl"] = new EntityProperty(this.CallbackUrl);
            return properties;
        }
    }   
}