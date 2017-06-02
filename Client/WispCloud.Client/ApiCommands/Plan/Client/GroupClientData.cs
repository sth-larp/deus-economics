using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class GroupClientData : ContainerClientData
    {
        public string Name { get; set; }
        public GroupSettings Settings { get; set; }
        public List<int> WindowIDs { get; set; }

    }

}