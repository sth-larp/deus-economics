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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(250)]
        public string Login { get; protected set; }

        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; }

        public string ParentID { get; set; }

        //[StringLength(80)]
        public string Fullname { get; set; }

        [Index("IX_Email", IsUnique = true)]
        [StringLength(80)]
        public string Email { get; set; }

        [Index("IX_Alias", IsUnique = true)]
        [StringLength(20)]
        public string Alias { get; set; }

        [Required]
        [JsonIgnore]
        public Guid TokenSalt { get; set; }

        [Required]
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
                if (Insurance == InsuranceType.Govt) return InsuranceLevel == 1 ? 0: 2;
                return InsuranceLevel;
            }
        }

        [Required]
        public float Cash { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        [JsonIgnore]
        public bool InsuranceHidden { get; set; }

        string IUser<string>.Id { get { return Login; } }

        string IUser<string>.UserName
        {
            get { return Login; }
            set { throw new NotImplementedException(); }
        }

        [DefaultValue(1)]
        public int InsuranceLevel { get; set; }


        [NotMapped]
        [JsonIgnore]
        public string DisplayName {
            get { return $"{Fullname} ({Login})"; } }

        [JsonIgnore]
        public float PaidSalary { get; set; }

        public Account()
        {
            Alias = Guid.NewGuid().ToString().Substring(0, 5);
            Email = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
        }

        public Account(string login, AccountRole role) : this()
        {
            Status = AccountStatus.Active;
            Login = login;
            Role = role;
            Insurance = InsuranceType.None;
        }
    }
}