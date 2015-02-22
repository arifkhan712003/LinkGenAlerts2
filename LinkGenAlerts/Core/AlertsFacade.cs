using System;

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
            try
            {
                //var data = _alerts.FetchData(_startDt, _endDt);

                //_alerts.AccumulateData(data);

                _alerts.RaiseAlerts(_startDt);

                Console.WriteLine("---------End----------");

            }
            catch (Exception exception)
            {
                Console.WriteLine( exception);
            }
        }
    }
}
