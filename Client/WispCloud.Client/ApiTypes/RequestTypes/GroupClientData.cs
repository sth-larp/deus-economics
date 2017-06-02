namespace WispCloudClient.ApiTypes
{
    public sealed class GroupClientData
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ParentGroupID { get; set; }
        public int[] GroupIDs { get; set; }
        public string[] PowerBarSNs { get; set; }

    }

}
