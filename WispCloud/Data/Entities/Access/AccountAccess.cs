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

        [NotMapped]
        public virtual string SlaveFullname { get { return $"{SlaveAccount.Fullname} ({Slave})"; } }

        [Key, Column(Order = 1)]
        [Index]
        public string Master { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Master")]
        public virtual Account MasterAccount { get; protected set; }

        [NotMapped]
        public virtual string MasterFullname { get { return $"{MasterAccount.Fullname} ({Master})"; } }

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


        //Для говнокода в GetAccessSlaves(string master)
        public AccountAccess(Account slave, Account master)
        {
            MasterAccount = master;
            Master = master.Login;
            SlaveAccount = slave;
            Slave = slave.Login;
        }

    }

}