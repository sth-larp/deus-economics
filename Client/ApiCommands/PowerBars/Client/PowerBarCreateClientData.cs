namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBarCreateClientData
    {
        public string PowerBarSN { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float DiagonalLTRB { get; set; }
        public float DiagonalRTLB { get; set; }
        public BarLocation BarLocation { get; set; }

    }

}