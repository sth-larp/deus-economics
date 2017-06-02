namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBar
    {
        public string PowerBarSN { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float DiagonalLTRB { get; set; }
        public float DiagonalRTLB { get; set; }
        public BarLocation BarLocation { get; set; }
        public int? ModeID { get; set; }
        public int[] TimerIDs { get; set; }
        public float Battery { get; set; }
        public float Shading { get; set; }
        public float UserBrightness { get; set; }
        public float RealBrightness { get; set; }
        public NetworkStatus Status { get; set; }

    }

}
