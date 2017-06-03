using System;
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
        public string Sender { get; set; }

        [JsonIgnore]
        [ForeignKey("Sender")]
        public virtual Account SenderAccount { get; set; }

        [Required]
        [Index]
        public string Receiver { get; set; }

        [JsonIgnore]
        [ForeignKey("Receiver")]
        public virtual Account ReceiverAccount { get; set; }

        public DateTime? LastPaid { get; set; }

        public TimeSpan? Period { get; set; }

        [Required]
        public float Debt { get; set; }

        [Required]
        public float Amount { get; set; }

        [Required]
        [JsonIgnore]
        public TransactionType Type { get; set; }

        [NotMapped]
        public IEnumerable<TransactionType> Flags { get { return Type.GetFlags(); } }

        public Payment()
        {
        }

        public Payment(Account sender, Account receiver, float amount)
        {
            SenderAccount = sender;
            ReceiverAccount = receiver;
            Amount = amount;
            Debt = 0;
            Type = TransactionType.Normal;
        }
    }
}