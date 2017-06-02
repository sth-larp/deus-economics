using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class MainContainerItem : ContainerItem
    {
        public List<FloorItem> Floors { get; set; }

    }

}