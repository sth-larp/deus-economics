using System;
using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities
{
    [DataContract]
    [Flags]
    public enum AccountAccessRoles
    {
        None = 0,
        Read = 1 << 0,
        Withdraw = 1 << 1,
        Admin = 1 << 2
    }

}