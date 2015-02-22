using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Model
{
    public class DownloadsThresholdConfig : TableEntity
    {
        public string SubscriberCode { get; set; }
        public string AlertAttributeId { get; set; }
        public int TriggerDurationInHours { get; set; }
        public int ThresholdValue { get; set; }
        public string AlertType { get; set; }
        public bool Enable { get; set; }
    }
}
