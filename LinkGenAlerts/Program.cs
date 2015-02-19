using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkGenAlerts.Core;
using LinkGenAlerts.Model;
using LinkGenAlerts.Repository;

namespace LinkGenAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadAzureStorageTable();
        }

        private static void ReadAzureStorageTable()
        {
            //2014070405
            //DateTime startDt = new DateTime(2015, 02, 11, 9, 0, 0);
            DateTime startDt = new DateTime(2014, 07, 04, 05, 0, 0);
            DateTime endDt = startDt.AddMinutes(60);

            IAlerts linkGenAlerts = new CdnAlerts(new AzureRepository());

            IAlertsFacade alertsFacade = new AlertsFacade(linkGenAlerts, startDt, endDt);
            alertsFacade.Execute();

            Console.ReadLine();
        }

    }
}
