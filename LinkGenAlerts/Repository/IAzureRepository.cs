using System;
using System.Collections.Generic;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Repository
{
    public interface IAzureRepository
    {
        IList<CdnRawData> FetchDownloadsData(DateTime downloadsFromTime, DateTime downloadsToTime);
        
        IList<AlertData> FetchAlerts(DateTime dateTime);
        
        List<DownloadsThresholdConfig> FetchThreshold();
        
        List<AlertAttribute> FetchAlertAttributes();
        
        void InsertDownloadsData(IList<DownloadsData> downloadsDatas);

        void InsertAlerts(List<AlertData> alertDatas);
    }
}
