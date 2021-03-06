﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Transactions
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Index]
        public string Employer { get; set; }

        [NotMapped]
        public string EmployerName { get { return $"{EmployerAccount.Fullname} ({Employer})"; } }

        [JsonIgnore]
        [ForeignKey("Employer")]
        public virtual Account EmployerAccount { get; set; }

        [Required]
        [Index]
        public string Receiver { get; set; }

        [JsonIgnore]
        [ForeignKey("Receiver")]
        public virtual Account ReceiverAccount { get; set; }

        [NotMapped]
        public string ReceiverName { get { return $"{ReceiverAccount.Fullname} ({Receiver})";} }

        public DateTime? LastPaid { get; set; }

        public TimeSpan? Period { get; set; }

        [Required]
        public int SalaryLevel { get; set; }

        [Required]
        [JsonIgnore]
        public TransactionType Type { get; set; }

        [NotMapped]
        public IEnumerable<TransactionType> Flags { get { return Type.GetFlags(); } }

        public Payment()
        {
        }

        public Payment(Account employer, Account receiver, int salaryLevel)
        {
            EmployerAccount = employer;
            ReceiverAccount = receiver;
            SalaryLevel = salaryLevel;
            Type = TransactionType.Normal;
        }
    }
}