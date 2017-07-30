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
        Company = 32,
    }

    public static class AccountRoleExtensions
    {
        public static bool IsCompany(this AccountRole role)
        {
            if (role == AccountRole.Company || role == AccountRole.Govt || role == AccountRole.Corp)
                return true;
            return false;

        }
    }
}