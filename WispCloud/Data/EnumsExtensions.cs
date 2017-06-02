using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Accounts;

namespace DeusCloud.Data
{
    public static class EnumsExtensions
    {
        public static IEnumerable<AccountRole> GetFlags(this AccountRole roles)
        {
            return Enum.GetValues(typeof(AccountRole))
                .Cast<AccountRole>()
                .Where(x => (roles & x) > 0);
        }

        public static IEnumerable<AccountAccessRoles> GetFlags(this AccountAccessRoles roles)
        {
            return Enum.GetValues(typeof(AccountAccessRoles))
                .Cast<AccountAccessRoles>()
                .Where(x => (roles & x) > 0);
        }

    }

}