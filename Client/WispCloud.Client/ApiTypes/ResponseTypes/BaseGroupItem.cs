using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class BaseGroupItem
    {
        public List<GroupItem> Groups { get; set; }
        public int? ModeID { get; set; }
        public int? MaxShading { get; set; }
        public int? MinShading { get; set; }
        public int? MaxUserBrightness { get; set; }
        public int? MinUserBrightness { get; set; }
        public int? MaxRealBrightness { get; set; }
        public int? MinRealBrightness { get; set; }
        public List<decimal> PowerBarSNs { get; set; }

    }

}
