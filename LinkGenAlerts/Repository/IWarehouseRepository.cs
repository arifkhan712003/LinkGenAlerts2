using System;
using System.Collections.Generic;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Repository
{
    public interface IWarehouseRepository
    {
        IList<IsapiDownloads> GetIsapiDownloadsInfo(DateTime downloadsFromTime, DateTime downloadsToTime);
    }
}
