namespace WispCloudClient.ApiTypes
{
    public sealed class HubSettingsClientData
    {
        public string HubSN { get; set; }
        public int? HeartbeatPeriod { get; set; }
        public bool EnableHotspot { get; set; }
    }

}