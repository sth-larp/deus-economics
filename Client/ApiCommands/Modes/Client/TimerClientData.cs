namespace WispCloudClient.ApiTypes
{
    public sealed class TimerClientData
    {
        public string Name { get; set; }
        public bool? IsShadingNull { get; set; }
        public float? Shading { get; set; }
        public bool? IsUserBrightnessNull { get; set; }
        public float? UserBrightness { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public bool? IsDaysNull { get; set; }

        /// <summary>
        /// Flags of days to repeat: MO = 1, TU = 2, WE = 4, TH = 8, FR = 16, SA = 32, SU = 64
        /// </summary>
        public int? Days { get; set; }

    }

}
