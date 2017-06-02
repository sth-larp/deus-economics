using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities
{
    public class AccountAccess
    {
        [Key, Column(Order = 0)]
        public long Slave { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Slave")]
        public virtual Account SlaveAccount { get; protected set; }

        [Key, Column(Order = 1)]
        public string Master { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Master")]
        public virtual Account MasterAccount { get; protected set; }

        [Required]
        public AccountAccessRoles Role { get; set; }

        [NotMapped]
        public IEnumerable<AccountAccessRoles> Roles { get { return Role.GetFlags(); } }

        public AccountAccess()
        {
        }

        public AccountAccess(Account master, Account slave, AccountAccessRoles role)
        {
            MasterAccount = master;
            SlaveAccount = slave;
            Role = role;
        }

    }

}