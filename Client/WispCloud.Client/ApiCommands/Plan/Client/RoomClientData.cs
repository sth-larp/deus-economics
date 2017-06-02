using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WispCloudClient.ApiTypes
{
    public sealed class RoomClientData : ContainerClientData
    {
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public RoomSettings Settings { get; set; }
        public List<WallClientData> Walls { get; set; }
        public List<GroupClientData> Groups { get; set; }

        [JsonIgnore]
        public override IEnumerable<ContainerClientData> Containers
        {
            get
            {
                if (Walls == null && Groups == null)
                    return null;

                var result = Enumerable.Empty<ContainerClientData>();
                if (Walls != null)
                    result = result.Concat(Walls);
                if (Groups != null)
                    result = result.Concat(Groups);

                return result;
            }
        }

    }

}