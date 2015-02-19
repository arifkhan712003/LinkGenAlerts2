namespace LinkGenAlerts.Model
{
    public class IsapiDownloads
    {
        public int SubscriberId { get; set; }

        public string SubscriberCode { get; set; }

        public long DownloadsCount { get; set; }

        public long? DownloadsVolumeInBytes { get; set; }
    }
}
