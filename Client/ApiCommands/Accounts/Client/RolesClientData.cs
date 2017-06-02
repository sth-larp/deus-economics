using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class RolesClientData
    {
        public string Login { get; set; }
        public List<AccountRoles> Roles { get; set; }

        /// <summary>
        /// Не использовать для выдачи прав
        /// </summary>
        /// // TODO: Сделать автоматически вычисляемым на базе списка ролей
        [JsonIgnore]
        public AccountRoles Role { get; set; }

        public bool? Active { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (Roles != null && Roles.Any())
                Role = Roles.Aggregate(AccountRoles.None, (role, next) => role |= next);
            else
                Role = AccountRoles.None;
        }

    }

}