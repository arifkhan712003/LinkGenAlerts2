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

        public virtual void AccumulateData(IList<DownloadsData> attributeData)
        {
            throw new NotImplementedException();
        }

        public virtual void RaiseAlerts()
        {
            throw new NotImplementedException();
        }

        private List<AlertData> GetAlerts()
        {
            throw new NotImplementedException();
        }
    }
}
