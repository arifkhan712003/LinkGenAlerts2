﻿using System;
using System.Collections.Generic;
using LinkGenAlerts.Model;

namespace LinkGenAlerts.Core
{
    public interface IAlerts
    {
        IList<DownloadsData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime);

        void AccumulateData(IList<DownloadsData> downloadsDatas);

        void RaiseAlerts(DateTime alertsTime);
    }
}
