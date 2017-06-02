using Newtonsoft.Json;
using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public class MainContainerClientData : ContainerClientData
    {
        public List<FloorClientData> Floors { get; set; }

        [JsonIgnore]
        public override IEnumerable<ContainerClientData> Containers { get { return Floors; } }

    }

}