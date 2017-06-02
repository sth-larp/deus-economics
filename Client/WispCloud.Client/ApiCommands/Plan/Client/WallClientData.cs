using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class WallClientData : ContainerClientData
    {
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public WallSettings Settings { get; set; }
        public List<int> WindowIDs { get; set; }

    }

}