using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class FloorItem : ContainerItem
    {
        public int SequenceNumber { get; set; }
        public List<RoomItem> Rooms { get; set; }

    }

}