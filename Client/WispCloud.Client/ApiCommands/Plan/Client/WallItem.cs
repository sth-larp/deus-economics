using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class WallItem : ContainerItem
    {
        public int SequenceNumber { get; set; }
        public WallSettings Settings { get; set; }
        public List<int> WindowIDs { get; set; }

    }

}