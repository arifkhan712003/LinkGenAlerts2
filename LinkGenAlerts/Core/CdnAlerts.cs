using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LinkGenAlerts.Model;
using LinkGenAlerts.Repository;
using LinkGenAlerts.Utillities;

namespace LinkGenAlerts.Core
{
    internal class CdnAlerts : AlertsBase
    {
        private readonly IAzureRepository _azureRepository;

        public CdnAlerts(IAzureRepository azureRepository) : base(azureRepository)
        {
            _azureRepository = azureRepository;
        }

        protected override int[] AttributeIds { get; set; }

        public override IList<DownloadsData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            downloadsFromTime = DateTime.SpecifyKind(downloadsFromTime, DateTimeKind.Utc);

            IList<CdnRawData> cdnRawDatas = GetLastUsage(downloadsFromTime, downloadsToTime);

            var cdnProcessedData = new CdnRawDataProcessor().GetAttributeData(cdnRawDatas);

            foreach (var downloadsData in cdnProcessedData)
            {
                downloadsData.PartitionKey = downloadsFromTime.ToString("yyyyMMdd");
                downloadsData.RowKey = Guid.NewGuid().ToString();
                downloadsData.FromTime = downloadsFromTime;
                downloadsData.ToTime = downloadsToTime;
                downloadsData.LastUpdatedOn = DateTime.UtcNow;
            }

            return cdnProcessedData.ToList();
        }

        private IList<CdnRawData> GetLastUsage(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            return _azureRepository.FetchData(downloadsFromTime, downloadsToTime);
        }

        public override void AccumulateData(IList<DownloadsData> attributeData)
        {
            _azureRepository.InsertAlerts(attributeData);
        }

        public override void RaiseAlerts(DateTime dateTime)
        {
            List<DownloadsThresholdConfig> thresholdConfigs = _azureRepository.FetchThreshold();

            List<DownloadsData> downloadsDatas = _azureRepository.FetchDownloadsData(dateTime);

            List<AlertAttribute> alertAttributes = _azureRepository.FetchAlertAttributes();

            var obj1 = from downloadsData in downloadsDatas
                group downloadsData by new {downloadsData.SubscriberId, downloadsData.AlertAttributeId}
                into downloadGroup
                join alertAttribute in alertAttributes
                    on downloadGroup.Key.AlertAttributeId equals alertAttribute.Id
                select
                    new AlertData()
                    {
                        SubscriberCode = downloadGroup.Key.SubscriberId,
                        AttributeName = alertAttribute.Name,
                        AttributeValue = downloadGroup.Sum(x => x.Value)
                    };


            foreach (var alertData in obj1)
            {
                Console.WriteLine(alertData.SubscriberCode +" "+ alertData.AttributeName +" "+ alertData.AttributeValue);
            }           

        }
    }
}
