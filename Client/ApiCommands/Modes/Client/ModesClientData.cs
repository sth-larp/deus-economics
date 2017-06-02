using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class ModesClientData
    {
        public Mode Automatic { get; set; }
        public Mode EnergySaving { get; set; }
        public List<Timer> Timers { get; set; }

    }

}
