using System;

namespace WispCloudClient.ApiTypes
{
    [Flags]
    public enum AccountRoles
    {
        Hub = 1 << 0,
        SeviceEnginier = 1 << 1,
        User = 1 << 2,
    }

}