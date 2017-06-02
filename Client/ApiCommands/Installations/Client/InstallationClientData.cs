using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class InstallationClientData
    {
        public long InstallationID { get; set; }
        public int SecurityKeyID { get; set; }
        public string EPID { get; set; }
        public string Name { get; set; }
        public List<InstallationRolesClientData> Accounts { get; set; }

    }

}
