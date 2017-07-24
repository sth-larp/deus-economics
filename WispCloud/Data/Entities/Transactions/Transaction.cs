using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Transactions
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Index]
        public string Sender { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Sender")]
        public virtual Account SenderAccount { get; protected set; }

        [Required]
        [Index]
        public string Receiver { get; protected set; }

        [JsonIgnore]
        [ForeignKey("Receiver")]
        public virtual Account ReceiverAccount { get; protected set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public float Amount { get; set; }

        public string Comment { get; set; }

        [Required]
        [JsonIgnore]
        public TransactionType Type { get; set; }

        [NotMapped]
        public IEnumerable<TransactionType> Flags { get { return Type.GetFlags(); } }

        public Transaction()
        {
        }

        public Transaction(Account sender, Account receiver, float amount)
        {
            SenderAccount = sender;
            ReceiverAccount = receiver;
            Amount = amount;
            Time = DateTime.Now;
        }

        public void HideIfRequired()
        {
            if ((Type & TransactionType.Anonymous) > 0)
            {
                Receiver = ReceiverAccount.Alias;
                Comment = Comment + " (анонимно)";
            }
        } 
    }
}