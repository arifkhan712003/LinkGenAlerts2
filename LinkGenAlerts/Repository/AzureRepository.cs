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

        public IList<CdnRawData> FetchDownloadsData(DateTime downloadsFromTime, DateTime downloadsToTime)
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

        public IList<AlertData> FetchAlerts(DateTime dateTime)
        {
            List<DownloadsThresholdConfig> thresholdConfigs = FetchThreshold();

            List<AlertData> rawAlertDatas = new List<AlertData>();

            //fore each subscriberid + attribute get alerts for tht many hours

            foreach (var threshold in thresholdConfigs)
            {
                if(threshold.AlertType.Equals("Warning"))
                    continue;

                var alertDatas = FetchAlertsForThreshold(dateTime, threshold);

                rawAlertDatas.AddRange(alertDatas); 
            }


            foreach (var returnAlertData in rawAlertDatas)
            {
                Console.WriteLine(returnAlertData.SubscriberCode +" "+ returnAlertData.AttributeName +" "+ returnAlertData.AttributeValue +" "+ returnAlertData.AlertType );
            }
            Console.WriteLine(">>>>");

            List<AlertData> finalAlertData = new List<AlertData>();

            foreach (var alertData in rawAlertDatas)
            {
                finalAlertData.AddRange(GetAlertsThatBreachedThreshold(alertData, thresholdConfigs, dateTime));
            }

            foreach (var returnAlertData in finalAlertData)
            {
                Console.WriteLine(">>>" + returnAlertData.SubscriberCode + " " + returnAlertData.AttributeValue +" "+ returnAlertData.AlertType);
            }

            return finalAlertData;
        }

        private static IEnumerable<AlertData> GetAlertsThatBreachedThreshold(AlertData alertData, List<DownloadsThresholdConfig> thresholdConfigs, DateTime dateTime)
        {
            var obj = (from threshold in thresholdConfigs
                where (threshold.SubscriberCode == alertData.SubscriberCode) &&
                      (threshold.AlertAttributeId == alertData.AttributeName) &&
                      (threshold.ThresholdValue <= alertData.AttributeValue)
                orderby threshold.ThresholdValue descending
                select
                    new AlertData()
                    {
                        SubscriberCode = threshold.SubscriberCode,
                        AttributeName = threshold.AlertAttributeId,
                        AttributeValue = alertData.AttributeValue,
                        ThresholdValue = threshold.ThresholdValue,
                        StartTime = dateTime,
                        EndTime = dateTime,
                        CreatedOn = DateTime.Now,
                        AlertType = threshold.AlertType
                    }).ToList().Take(1);
            return obj;
        }

        private IEnumerable<AlertData> FetchAlertsForThreshold(DateTime dateTime, DownloadsThresholdConfig threshold)
        {
            var whereFilter = GetFilterConditionForGeneratingAlerts(dateTime, threshold);

            TableQuery<DownloadsData> tableQuery = new TableQuery<DownloadsData>().Where(whereFilter);

            IList<DownloadsData> downloadsDatas = GetCloudTable("DownloadsData").ExecuteQuery(tableQuery).ToList();

            return GetAggregatedAlerts(downloadsDatas);
        }

        private static IEnumerable<AlertData> GetAggregatedAlerts(IList<DownloadsData> downloadsDatas)
        {
            var alertDatas = from downloadsData in downloadsDatas
                group downloadsData by new {downloadsData.SubscriberId, downloadsData.AlertAttributeId}
                into downloadGroup
                select
                    new AlertData()
                    {
                        SubscriberCode = downloadGroup.Key.SubscriberId,
                        AttributeName = downloadGroup.Key.AlertAttributeId,
                        AttributeValue = downloadGroup.Sum(x => x.Value)
                    };
            return alertDatas;
        }

        private static string GetFilterConditionForGeneratingAlerts(DateTime dateTime, DownloadsThresholdConfig threshold)
        {
            string partitionKeyFilter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, dateTime.ToString("yyyyMMdd")),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    dateTime.AddDays(-1).ToString("yyyyMMdd")));

            string subscriberFilter = TableQuery.GenerateFilterCondition("SubscriberId", QueryComparisons.Equal,
                threshold.SubscriberCode);

            string attributeFilter = TableQuery.GenerateFilterCondition("AlertAttributeId", QueryComparisons.Equal,
                threshold.AlertAttributeId);

            string durationFilter = TableQuery.GenerateFilterConditionForDate("FromTime",
                QueryComparisons.GreaterThanOrEqual, dateTime.AddHours(-threshold.TriggerDurationInHours));

            string whereFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, subscriberFilter);
            whereFilter = TableQuery.CombineFilters(whereFilter, TableOperators.And, attributeFilter);
            whereFilter = TableQuery.CombineFilters(whereFilter, TableOperators.And, durationFilter);
            
            return whereFilter;
        }

        private CloudTable GetCloudTable(string tableName)
        {
            return GetCloudTableClient().GetTableReference(tableName);
        }

        private CloudTableClient GetCloudTableClient()
        {
            return CloudStorageAccount.Parse(_connectionString).CreateCloudTableClient();
        }


        private IList<string> FetchTenentTables()
        {
            IEnumerable<CloudTable> tableList = GetCloudTableClient().ListTables();

            List<string> tableNames = tableList.Select(ct => ct.Name).ToList();

            tableNames.RemoveAll(x => _diagnosticTables.Contains(x));

            return tableNames;
        }


        private string GetFilterCondition(DateTime downloadsFromTime, DateTime downloadsToTime)
        {

            string fromTimeFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual,
                downloadsFromTime);

            string toTimeFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual,
                downloadsToTime);

            string filterCondition = TableQuery.CombineFilters(fromTimeFilter, TableOperators.And, toTimeFilter);

            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                downloadsFromTime.ToString("yyyyMMddHH"));

            filterCondition = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, filterCondition);

            return filterCondition;
        }

        public List<DownloadsThresholdConfig> FetchThreshold()
        {
            CloudTable table = GetCloudTable("DownloadsThresholdConfig");

            TableQuery<DownloadsThresholdConfig> tableQuery = new TableQuery<DownloadsThresholdConfig>();

            return table.ExecuteQuery(tableQuery).ToList();
        }

        public List<AlertAttribute> FetchAlertAttributes()
        {
            var table = GetCloudTable("AlertAttribute");

            TableQuery<AlertAttribute> query = new TableQuery<AlertAttribute>();

            return table.ExecuteQuery(query).ToList();
        }

        public void InsertDownloadsData(IList<DownloadsData> downloadsDatas)
        {
            CloudTable table = GetCloudTable("DownloadsData");
            table.CreateIfNotExists();

            foreach (var downloadsData in downloadsDatas)
            {
                TableOperation operation = TableOperation.Insert(downloadsData);
                table.Execute(operation);
            }
        }

        public void InsertAlerts(List<AlertData> alertDatas)
        {
            CloudTable table = GetCloudTable("AlertData");
            table.CreateIfNotExists();

            foreach (var alertData in alertDatas)
            {
                TableOperation operation = TableOperation.Insert(alertData);
                table.Execute(operation);
            }
        }
    }
}
