namespace WispCloudClient.ApiTypes
{
    public sealed class ContainerItemStatus
    {
        public int? ModeID { get; set; }
        public float? MaxShading { get; set; }
        public float? MinShading { get; set; }
        public float? MaxUserBrightness { get; set; }
        public float? MinUserBrightness { get; set; }
        public float? MaxRealBrightness { get; set; }
        public float? MinRealBrightness { get; set; }

    }

}