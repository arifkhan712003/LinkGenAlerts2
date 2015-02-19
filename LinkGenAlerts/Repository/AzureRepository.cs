using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinkGenAlerts.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Repository
{
    internal class AzureRepository : IAzureRepository
    {
        readonly string _connectionString;

        readonly List<String> _diagnosticTables = new List<string>()
            {
                "WADDiagnosticInfrastructureLogsTable",
                "WADDirectoriesTable",
                "WADLogsTable",
                "WADWindowsEventLogsTable"
            };

        public AzureRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
        }

        public IList<CdnRawData> FetchData(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            List<CdnRawData> cdnRawDatas = new List<CdnRawData>();

            var tableClient = GetCloudTableClient();

            foreach (var tenentTable in FetchTenentTables())
            {
                CloudTable table = tableClient.GetTableReference(tenentTable);

                TableQuery<CdnRawData> query = new TableQuery<CdnRawData>().Where(GetFilterCondition(downloadsFromTime, downloadsToTime));

                cdnRawDatas.AddRange(table.ExecuteQuery(query).ToList());
            }

            return cdnRawDatas;
        }

        private CloudTableClient GetCloudTableClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

            return storageAccount.CreateCloudTableClient();
        }


        private IList<string> FetchTenentTables()
        {
            IEnumerable<CloudTable> tableList = GetCloudTableClient().ListTables();

            List<string> tableNames = tableList.Select(ct => ct.Name).ToList();

            tableNames.RemoveAll(x => _diagnosticTables.Contains(x));

            return tableNames;
        }


        private static string GetFilterCondition(DateTime downloadsFromTime, DateTime downloadsToTime)
        {
            downloadsFromTime = DateTime.SpecifyKind(downloadsFromTime, DateTimeKind.Utc);

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                downloadsFromTime.ToString("yyyyMMddHH"));

            DateTimeOffset endOffSet = DateTime.SpecifyKind(downloadsToTime,DateTimeKind.Utc);

            string startTime = TableQuery.GenerateFilterConditionForDate("Timestamp",
                QueryComparisons.GreaterThanOrEqual, downloadsFromTime);
            string endtime = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual,
                endOffSet);

            string filterCondition = TableQuery.CombineFilters(startTime, TableOperators.And, endtime);
            filterCondition = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, filterCondition);

            return filterCondition;
        }

        public void InsertAlerts(IList<DownloadsData> attributeData)
        {
            CloudTableClient tableClient = GetCloudTableClient();
            CloudTable table = tableClient.GetTableReference("DownloadsData");
            table.CreateIfNotExists();

            foreach (var downloadsData in attributeData)
            {
                TableOperation operation = TableOperation.Insert(downloadsData);
                table.Execute(operation);
            }
        }
    }
}
