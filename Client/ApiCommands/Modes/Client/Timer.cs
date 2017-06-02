namespace WispCloudClient.ApiTypes
{
    public sealed class Timer
    {
        public int TimerID { get; set; }
        public string Name { get; set; }
        public float? Shading { get; set; }
        public float? UserBrightness { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }

        /// <summary>
        /// Flags of days to repeat: MO = 1, TU = 2, WE = 4, TH = 8, FR = 16, SA = 32, SU = 64
        /// </summary>
        public int? Days { get; set; }

    }

}
