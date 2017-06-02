using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class PlanClientData
    {
        public MainContainerClientData MainContainer { get; set; }
        public List<WindowClientData> Windows { get; set; }

    }

}