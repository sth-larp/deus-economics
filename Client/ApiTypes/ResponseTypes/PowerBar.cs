namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBar
    {
        public string PowerBarSN { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int? ModeID { get; set; }

        public int Battery { get; set; }

        public int Shading { get; set; }

        public int UserBrightness { get; set; }

        public int RealBrightness { get; set; }

        public NetworkStatus Status { get; set; }

    }

}
