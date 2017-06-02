using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class ButtonWallSwitchUserClientData
    {
        public string Name { get; set; }
        public bool? IsFirstPresetNull { get; set; }
        public ButtonWallSwitchSettings FirstPreset { get; set; }
        public bool? IsSecondPresetNull { get; set; }
        public ButtonWallSwitchSettings SecondPreset { get; set; }
        public bool? IsModeIDNull { get; set; }
        public int? ModeID { get; set; }
        public List<ContainerToButtonWallSwitchClientData> Containers { get; set; }

    }

}