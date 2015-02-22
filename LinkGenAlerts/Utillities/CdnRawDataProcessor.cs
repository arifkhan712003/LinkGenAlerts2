using System;
using System.Collections.Generic;
using System.Linq;
using LinkGenAlerts.Core;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Utillities
{
    public class CdnRawDataProcessor
    {
        public List<DownloadsData> GetAttributeData(IList<CdnRawData> cdnRawDatas)
        {
            List<DownloadsData> cdnProcessedData = GetFileCountAttribute(cdnRawDatas);

            return cdnProcessedData;
        }

        private List<DownloadsData> GetFileCountAttribute(IList<CdnRawData> cdnRawDatas)
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