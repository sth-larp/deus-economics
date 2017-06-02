using Newtonsoft.Json;
using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public abstract class ContainerClientData
    {
        public int ContainerID { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<ContainerClientData> Containers { get { return null; } }

    }

}