using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Accounts
{
    public abstract class Account : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(250)]
        public string Login { get; protected set; }


        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; }

        [Required]
        [JsonIgnore]
        public Guid TokenSalt { get; set; }

        [Required]
        public AccountRole Role { get; set; }

        [Required]
        public float Cash { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        [JsonIgnore]
        public virtual List<AccountAccess> AccountAccesses { get; set; }

        string IUser<string>.Id { get { return Login; } }

        string IUser<string>.UserName
        {
            get { return Login; }
            set { throw new NotImplementedException(); }
        }

        public Account()
        {
        }

        public Account(string login, AccountRole role)
        {
            this.Status = AccountStatus.Active;
            this.Login = login;
            this.Role = role;
        }

    }

}