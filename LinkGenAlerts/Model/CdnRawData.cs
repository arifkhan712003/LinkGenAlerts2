using Microsoft.WindowsAzure.Storage.Table;

namespace LinkGenAlerts.Model
{
    public class CdnRawData : TableEntity
    {
        public string ClientInformation_Name { get; set; }

        public string DSLinkGenRequest_EndUserIPAddress { get; set; }
    }
}