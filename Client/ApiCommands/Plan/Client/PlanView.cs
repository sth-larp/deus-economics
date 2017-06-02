using System.Collections.Generic;

namespace WispCloudClient.ApiTypes
{
    public sealed class PlanView
    {
        public MainContainerItem MainContainer { get; set; }

        public List<WindowItem> Windows { get; set; }

    }

}