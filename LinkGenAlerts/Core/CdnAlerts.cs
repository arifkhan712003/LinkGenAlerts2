using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LinkGenAlerts.Model;
using LinkGenAlerts.Repository;
using LinkGenAlerts.Utillities;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Core
{
    internal class CdnAlerts : AlertsBase
    {
        private readonly IAzureRepository _azureRepository;

        public CdnAlerts(IAzureRepository azureRepository)
            : base(azureRepository)
        {
            _azureRepository = azureRepository;
        }

        protected override int[] AttributeIds { get; set; }

        public override IList<DownloadsData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            IList<CdnRawData> cdnRawDatas = GetLastUsage(downloadsFromTime, downloadsToTime);

            IList<DownloadsData> cdnProcessedData = new CdnRawDataProcessor().GetDataForAllAttributes(cdnRawDatas);

            foreach (var downloadsData in cdnProcessedData)
            {
                downloadsData.PartitionKey = downloadsFromTime.ToString("yyyyMMdd");
                downloadsData.RowKey = Guid.NewGuid().ToString();
                downloadsData.FromTime = downloadsFromTime;
                downloadsData.ToTime = downloadsToTime;
                downloadsData.LastUpdatedOn = DateTime.UtcNow;
            }

            return cdnProcessedData;
        }

        private IList<CdnRawData> GetLastUsage(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            return _azureRepository.FetchDownloadsData(downloadsFromTime, downloadsToTime);
        }

        public override void AccumulateData(IList<DownloadsData> downloadsDatas)
        {
            _azureRepository.InsertDownloadsData(downloadsDatas);
        }

        public override void RaiseAlerts(DateTime dateTime)
        {
            List<DownloadsThresholdConfig> thresholdConfigs = _azureRepository.FetchThreshold();

            IList<AlertData> downloadsDatas = _azureRepository.FetchAlerts(dateTime);

            /*
            var alertDatas = from downloadsData in downloadsDatas
                             group downloadsData by new { downloadsData.SubscriberId, downloadsData.AlertAttributeId }
                                 into downloadGroup
                                 select
                                     new AlertData()
                                     {
                                         SubscriberCode = downloadGroup.Key.SubscriberId,
                                         AttributeName = downloadGroup.Key.AlertAttributeId,
                                         AttributeValue = downloadGroup.Sum(x => x.Value)
                                     };

            List<AlertData> returnAlertDatas = new List<AlertData>();

            foreach (var alertData in alertDatas)
            {
                var obj = (from threshold in thresholdConfigs
                           where (threshold.SubscriberCode == alertData.SubscriberCode) &&
                                 (threshold.AlertAttributeId == alertData.AttributeName) &&
                                 (threshold.ThresholdValue <= alertData.AttributeValue)
                           orderby threshold.ThresholdValue descending
                           select
                               new AlertData()
                               {
                                   SubscriberCode = threshold.SubscriberCode,
                                   AttributeName = threshold.AlertAttributeId,
                                   AttributeValue = alertData.AttributeValue,
                                   ThresholdValue = threshold.ThresholdValue,
                                   StartTime = dateTime,
                                   EndTime = dateTime,
                                   CreatedOn = DateTime.Now

                               }).ToList().Take(1);

                returnAlertDatas.AddRange(obj);

                Console.WriteLine(alertData.SubscriberCode + " " + alertData.AttributeName + " " + alertData.AttributeValue);
            }

            foreach (var returnAlertData in returnAlertDatas)
            {
                Console.WriteLine(">>>" + returnAlertData.SubscriberCode + " " + returnAlertData.AttributeValue);
            }

            _azureRepository.InsertAlerts(returnAlertDatas);
             */ 
        }
    }
}
