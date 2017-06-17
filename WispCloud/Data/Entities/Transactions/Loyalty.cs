﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Transactions
{
    public class Loyalty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Index]
        public InsuranceType Insurance { get; set; }

        [Required]
        [Index]
        public string LoyalName { get; set; }

        [JsonIgnore]
        [ForeignKey("LoyalName")]
        public virtual Account LoyalService { get; set; }

        public Loyalty()
        {
        }

        public Loyalty(Account service, InsuranceType type)
        {
            LoyalService = service;
            Insurance = type;
        }
    }
}