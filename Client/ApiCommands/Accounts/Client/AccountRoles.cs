﻿using System;

namespace WispCloudClient.ApiTypes
{
    [Flags]
    public enum AccountRoles
    {
        None = 0,
        Hub = 1 << 0,
        SeviceEnginier = 1 << 1,
        User = 1 << 2,
        Installer = 1 << 3,

    }

}