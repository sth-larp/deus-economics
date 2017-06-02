namespace WispCloud.Logic.Groups
{
    public sealed class PowerBarUserClientData : PowerBarActionClientData
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int[] GroupIDs { get; set; }

    }

}