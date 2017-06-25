using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Serialization;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Accounts
{
    public class Account : IUser
    {
        //[NotMapped]
        //public UserSettings Settings { get; set; }

        //[JsonIgnore]
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public string SettingsJson
        //{
        //    get { return WispJsonSerializer.SerializeToJsonString(Settings); }
        //    set { Settings = WispJsonSerializer.DeserializeJson<UserSettings>(value); }
        //}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(250)]
        public string Login { get; protected set; }


        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; }

        public string Fullname { get; set; }

        [Required]
        [JsonIgnore]
        public Guid TokenSalt { get; set; }

        [Required]
        [JsonIgnore]
        public AccountRole Role { get; set; }

        [Required]
        public int Index { get; set; }

        public int InsurancePoints { get; set; }

        public InsuranceType Insurance { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int EffectiveLevel 
        {
            get
            {
                if (Insurance == InsuranceType.None) return 0;
                if (Insurance == InsuranceType.SuperVip) return 3;
                return InsuranceLevel;
            }
        }

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

        [DefaultValue(1)]
        public int InsuranceLevel { get; set; }

        public Account()
        {
        }

        public Account(string login, AccountRole role)
        {
            Status = AccountStatus.Active;
            Login = login;
            Role = role;
            Insurance = InsuranceType.None;
        }
    }
}