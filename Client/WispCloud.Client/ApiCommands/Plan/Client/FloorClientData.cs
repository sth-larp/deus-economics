using Newtonsoft.Json;
using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class FloorClientData : ContainerClientData
    {
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public List<RoomClientData> Rooms { get; set; }

        [JsonIgnore]
        public override IEnumerable<ContainerClientData> Containers { get { return Rooms; } }

    }

}