﻿using System;

namespace LinkGenAlerts.Core
{
    internal class AlertsFacade : IAlertsFacade
    {
        private readonly IAlerts _alerts;
        private readonly DateTime _startDt;
        private readonly DateTime _endDt;

        public AlertsFacade(IAlerts alerts)
        {
            this._alerts = alerts;
        }

        public AlertsFacade(IAlerts linkGenAlerts, DateTime startDt, DateTime endDt)
        {
            this._alerts = linkGenAlerts;
            this._startDt = startDt;
            this._endDt = endDt;
        }

        public void Execute()
        {
            var data = _alerts.FetchData(_startDt, _endDt);

            foreach (var downloadsData in data)
            {
                Console.WriteLine(downloadsData.SubscriberId+" "+ downloadsData.Value +" "+ downloadsData.FromTime);
            }
            
            _alerts.AccumulateData(data);
            //this._alerts.RaiseAlerts();
        }
    }
}