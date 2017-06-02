namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBarStatusClientData
    {
        public string PowerBarSN { get; set; }
        public float Battery { get; set; }
        public float RealBrightness { get; set; }
        public int? Radio { get; set; }
    }

}