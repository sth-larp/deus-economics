namespace WispCloudClient.ApiTypes
{
    public sealed class ButtonWallSwitchUserClientData
    {
        public string Name { get; set; }
        public bool? IsFirstPresetNull { get; set; }
        public ButtonWallSwitchSettings FirstPreset { get; set; }
        public bool? IsSecondPresetNull { get; set; }
        public ButtonWallSwitchSettings SecondPreset { get; set; }
        public int? ModeID { get; set; }
        public int[] GroupIDs { get; set; }

    }

}
