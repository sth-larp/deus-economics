using System;

namespace WispCloudClient.ApiTypes
{
    [Flags]
    public enum DaysOfWeek
    {
        Monday = 1 << 0,
        Tuesday = 1 << 1,
        Wednesday = 1 << 2,
        Thursday = 1 << 3,
        Friday = 1 << 4,
        Saturday = 1 << 5,
        Sunday = 1 << 6,
    }

    public sealed class TimerModeSettings
    {
        public int Shading { get; set; }
        public int UserBrightness { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        DaysOfWeek RepeatDays { get; set; }

    }

}
