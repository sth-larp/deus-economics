namespace WispCloudClient.ApiTypes
{
    public sealed class GroupItem : BaseGroupItem
    {
        public int GroupID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsMainGroupInInstallation { get; set; }

    }

}
