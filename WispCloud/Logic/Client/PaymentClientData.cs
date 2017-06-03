using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class PaymentClientData : BaseModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public float Amount { get; set; }
        public override void Validate()
        {
            Try.NotEmpty(Sender, $"Sender can't be empty.");
            Try.NotEmpty(Receiver, $"Receiver can't be empty.");
            Try.Condition(Amount > 0, $"Amount must be positive");
        }
    }
}