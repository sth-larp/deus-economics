using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public class TransferClientData : BaseModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public float Amount { get; set; }
        public string Description { get; set; }

        public TransferClientData()
        {
        }

        public TransferClientData(string sender, string receiver, float amount)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Description = "";
        }
        public override void Validate()
        {
            Try.NotEmpty(Sender, $"Поле {nameof(Sender)} не должно быть пустым");
            Try.NotEmpty(Receiver, $"Поле {nameof(Receiver)} не должно быть пустым");
            Try.Condition(Amount >= 1, $"Сумма транзакции должна быть не менее 1 кр");
        }
    }
}