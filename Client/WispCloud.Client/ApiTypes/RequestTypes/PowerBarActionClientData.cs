namespace WispCloudClient.ApiTypes
{
    public class PowerBarActionClientData
    {
        public int? SourceModeID { get; set; }
        public decimal? SourceButtonWallSwitchSN { get; set; }
        public PowerBarActionParamsClientData[] PowerBarsParams { get; set; }

    }

}
