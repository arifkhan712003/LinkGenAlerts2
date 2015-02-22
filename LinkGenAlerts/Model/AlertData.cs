using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Model
{
    public class AlertData : TableEntity
    {
        public AlertData()
        {
            PartitionKey = DateTime.Now.ToString("yyyyMMdd");
            RowKey = Guid.NewGuid().ToString();
        }

        public string SubscriberCode { get; set; }

        public string AttributeName { get; set; }

        public long AttributeValue { get; set; }

        public long ThresholdValue { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreatedOn { get; set; }

        public string AlertType { get; set; }
    }
}
