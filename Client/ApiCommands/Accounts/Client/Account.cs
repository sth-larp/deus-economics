using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class Account
    {
        public string Login { get; set; }
        public List<AccountRoles> Roles { get; set; }
        public AccountRoles Role { get; set; }
        public UserSettings Settings { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Role = Roles.Aggregate(AccountRoles.None, (role, next) => role |= next);
        }

    }

}
