using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class PowerBarActionClientData
    {
        public int? SourceModeID { get; set; }
        public string SourceButtonWallSwitchSN { get; set; }
        public List<PowerBarActionParamsClientData> PowerBarsParams { get; set; }

    }

}
