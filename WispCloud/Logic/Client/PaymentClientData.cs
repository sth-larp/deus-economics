using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class PaymentClientData : BaseModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int SalaryLevel { get; set; }
        public override void Validate()
        {
            Try.NotEmpty(Sender, $"Sender can't be empty.");
            Try.NotEmpty(Receiver, $"Receiver can't be empty.");
            Try.Condition(SalaryLevel > 0, $"Salary level must be positive");
        }
    }
}