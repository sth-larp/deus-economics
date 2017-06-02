namespace WispCloudClient.ApiTypes
{
    public abstract class ContainerItem
    {
        public int ContainerID { get; set; }
        public string Name { get; set; }
        public ContainerItemStatus Status { get; set; }

    }

}