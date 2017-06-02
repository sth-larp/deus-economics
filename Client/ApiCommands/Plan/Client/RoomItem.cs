using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class RoomItem : ContainerItem
    {
        public int SequenceNumber { get; set; }
        public RoomSettings Settings { get; set; }
        public List<WallItem> Walls { get; set; }
        public List<GroupItem> Groups { get; set; }

    }

}