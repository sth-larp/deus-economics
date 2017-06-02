using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Rights.Client
{
    public sealed class AccountAccessClientData : BaseModel
    {
        public string MasterLogin { get; set; }
        public IEnumerable<AccountAccessRoles> Roles { get; set; }

        public AccountAccessClientData()
        {
        }

        public AccountAccessClientData(AccountAccess access)
        {
            this.MasterLogin = access.Master;
            this.Roles = access.Roles;
        }

        public override void Validate()
        {
            Try.NotEmpty(MasterLogin, $"{nameof(MasterLogin)} cant be empty.");
            Try.Condition(Roles != null && Roles.Any(x => x != AccountAccessRoles.None),
                $"{nameof(Roles)} cant be empty.");
        }

    }

}