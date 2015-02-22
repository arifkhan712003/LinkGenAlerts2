using System;
using System.Collections.Generic;
using System.Linq;
using LinkGenAlerts.Core;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Utillities
{
    public class CdnRawDataProcessor
    {
        public IList<DownloadsData> GetDataForAllAttributes(IList<CdnRawData> cdnRawDatas)
        {
            IList<DownloadsData> cdnProcessedData = GetFileCountAttribute(cdnRawDatas);

            return cdnProcessedData;
        }

        private IList<DownloadsData> GetFileCountAttribute(IList<CdnRawData> cdnRawDatas)
        {

            return (from rawData in cdnRawDatas
                group rawData by rawData.ClientInformation_Name
                into groupedRawData
                        select new DownloadsData
                {
                    SubscriberId = groupedRawData.Key,
                    AlertAttributeId = AlertAttributeTypes.DownloadsCount,
                    Value = groupedRawData.Count()
                }).ToList();
        }

        private IEnumerable<DownloadsData> GetDownloadVolumeAttribute(IList<CdnRawData> cdnRawDatas)
        {
            return (from rawData in cdnRawDatas
                group rawData by rawData.ClientInformation_Name
                into groupedRawData
                        select new DownloadsData
                {
                    //SubscriberId = groupedRawData.Key,
                    //FileSizeInBytes = groupedRawData.Sum(x => x.FileSizeInBytes)
                }).ToList();
        }

    }
}