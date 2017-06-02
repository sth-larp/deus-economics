using System;
using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Accounts
{
    [Flags]
    [DataContract]
    public enum AccountRole
    {
        None = 0,
        Person = 1,
        Master = 2,
        Admin = 4,
        Corp = 8,
        Govt = 16,
    }

}