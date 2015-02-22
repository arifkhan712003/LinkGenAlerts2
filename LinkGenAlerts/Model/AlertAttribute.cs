using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Model
{
    public class AlertAttribute : TableEntity
    {
        public string Id
        {
            get
            {
                return RowKey;
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
