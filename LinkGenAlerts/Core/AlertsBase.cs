using System;
using System.Collections.Generic;
using LinkGenAlerts.Model;
using LinkGenAlerts.Repository;

namespace LinkGenAlerts.Core
{
    internal abstract class AlertsBase : IAlerts
    {
        protected AlertsBase(IWarehouseRepository warehouseRepository)
        {
            this.WarehouseRepository = warehouseRepository;
        }

        protected AlertsBase(IAzureRepository azureRepository)
        {
            this.AzureRepository = azureRepository;
        }

        public IAzureRepository AzureRepository { get; set; }

        protected abstract int[] AttributeIds { get; set; }

        protected IWarehouseRepository WarehouseRepository { get; private set; }

        public abstract IList<DownloadsData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime);

        public virtual void AccumulateData(IList<DownloadsData> downloadsDatas)
        {
            throw new NotImplementedException();
        }

        public virtual void RaiseAlerts(DateTime alertsTime)
        {
            // Get Alerts
            IList<AlertData> alertData = this.GetAlerts(alertsTime);
            
            // Write Alerts into database
            this.WarehouseRepository.WriteAlerts(alertData);
        }

        private IList<AlertData> GetAlerts(DateTime alertsTime)
        {
            IList<AlertData> alertData = new List<AlertData>();
            alertData = this.WarehouseRepository.GetAlerts(alertsTime);
            return alertData;
        }
    }
}
