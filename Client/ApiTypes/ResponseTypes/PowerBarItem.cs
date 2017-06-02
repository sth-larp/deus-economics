using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class PowerBarItem
    {
        public string PowerBarSN { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ModeID { get; set; }
        public int Battery { get; set; }
        public int Shading { get; set; }
        public int RealBrightness { get; set; }
        public int UserBrightness { get; set; }
        public NetworkStatus Status { get; set; }
        public List<int> GroupIDs { get; set; }

    }

}
