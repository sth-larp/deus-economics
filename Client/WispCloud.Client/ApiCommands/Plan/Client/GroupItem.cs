using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class GroupItem : ContainerItem
    {
        public GroupSettings Settings { get; set; }
        public List<int> WindowIDs { get; set; }

    }

}