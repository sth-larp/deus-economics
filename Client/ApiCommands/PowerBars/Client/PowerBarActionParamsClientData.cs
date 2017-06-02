namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBarActionParamsClientData
    {
        public string PowerBarSN { get; set; }
        public bool? IsModeIDNull { get; set; }
        public int? ModeID { get; set; }
        public int[] TimerIDs { get; set; }
        public float? Shading { get; set; }
        public float? UserBrightness { get; set; }

    }

}
