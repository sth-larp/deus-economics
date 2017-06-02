using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Serialization;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Accounts
{
    public class Account : IUser
    {
        [NotMapped]
        public UserSettings Settings { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SettingsJson
        {
            get { return WispJsonSerializer.SerializeToJsonString(Settings); }
            set { Settings = WispJsonSerializer.DeserializeJson<UserSettings>(value); }
        }

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
        [JsonIgnore]
        public AccountRole Role { get; set; }

        [NotMapped]
        public IEnumerable<AccountRole> Roles { get { return Role.GetFlags(); } }

        [Required]
        public float Cash { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

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
            Status = AccountStatus.Active;
            Login = login;
            Role = role;
        }

    }

}