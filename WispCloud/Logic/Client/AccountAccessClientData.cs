using System.Collections.Generic;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class AccountAccessClientData : BaseModel
    {
        public string MasterLogin { get; set; }
        public string SlaveLogin { get; set; }
        public IEnumerable<AccountAccessRoles> Roles { get; set; }

        public AccountAccessClientData()
        {
        }

        public AccountAccessClientData(AccountAccess access)
        {
            MasterLogin = access.Master;
            SlaveLogin = access.Slave;
            Roles = access.Roles;
        }

        public override void Validate()
        {
            Try.NotEmpty(MasterLogin, $"{nameof(MasterLogin)} cant be empty.");
            Try.NotEmpty(SlaveLogin, $"{nameof(SlaveLogin)} cant be empty.");
            Try.Condition(Roles != null, $"{nameof(Roles)} cant be null.");
        }

    }

}