using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Model
{
    public class DownloadsData :TableEntity
    {
        //public DownloadsData()
        //{

        //}

        //public DownloadsData(string partitionKey, string rowKey)
        //    : base(partitionKey, rowKey)
        //{
        //}

        public string SubscriberId { get; set; }
        public string AlertAttributeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int Value { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}
