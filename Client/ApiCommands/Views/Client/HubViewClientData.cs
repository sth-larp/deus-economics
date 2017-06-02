using System.Collections.Generic;
using WispCloudClient.ApiTypes;

namespace WispCloud.Logic
{
    public sealed class HubViewClientData
    {
        public List<PowerBar> PowerBars { get; set; }
        public ModesClientData Modes { get; set; }
        public List<ButtonWallSwitch> ButtonWallSwitches { get; set; }
        public List<ContainerPowerBarSNsClientData> ContainersPowerBarSNs { get; set; }

    }

}