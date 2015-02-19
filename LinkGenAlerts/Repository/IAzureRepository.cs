using System;
using System.Collections.Generic;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Repository
{
    public interface IAzureRepository
    {
        IList<CdnRawData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime);

        void InsertAlerts(IList<DownloadsData> attributeData);
    }
}
