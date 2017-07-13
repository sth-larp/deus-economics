using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Access
{
    public class AccountAccess
    {
        [Key, Column(Order = 0)]
        [Index]
        public string Slave { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Slave")]
        public virtual Account SlaveAccount { get; protected set; }

        [Key, Column(Order = 1)]
        [Index]
        public string Master { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Master")]
        public virtual Account MasterAccount { get; protected set; }

        [Required]
        public AccountAccessRoles Role { get; set; }

        public AccountAccess()
        {
        }

        public AccountAccess(Account slave, Account master, AccountAccessRoles roles)
        {
            MasterAccount = master;
            SlaveAccount = slave;
            Role = roles;
        }

    }

}