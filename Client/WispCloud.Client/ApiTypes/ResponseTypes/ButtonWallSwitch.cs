using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class ButtonWallSwitch
    {
        public string ButtonWallSwitchSN { get; set; }
        public string Name { get; set; }
        public ButtonWallSwitchSettings FirstPreset { get; set; }
        public ButtonWallSwitchSettings SecondPreset { get; set; }
        public int? ModeID { get; set; }
        public int Battery { get; set; }
        public NetworkStatus Status { get; set; }
        public List<int> GroupIDs { get; set; }

    }

}
