using System;
using System.Collections.Generic;
using System.Linq;
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
            IList<CdnRawData> cdnRawDatas = GetLastUsage(downloadsFromTime, downloadsToTime);

            var cdnProcessedData = new CdnRawDataProcessor().GetAttributeData(cdnRawDatas);

            foreach (var downloadsData in cdnProcessedData)
            {
                downloadsData.PartitionKey = DateTime.UtcNow.ToString("yyyyMMddHH");
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
    }
}
