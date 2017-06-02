using System;
using System.Runtime.Serialization;

namespace WispCloud.Logic
{
    [DataContract]
    [Flags]
    public enum InInstallationRoles
    {
        Hub = 1 << 0,
        Administrator = 1 << 1,
        User = 1 << 2,
    }

}