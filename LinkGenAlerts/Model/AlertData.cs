using System;

namespace LinkGenAlerts.Model
{
    public class AlertData
    {
        public string SubscriberCode { get; set; }

        public string AttributeName { get; set; }

        public long AttributeValue { get; set; }

        public long ThresholdValue { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
