using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public class ImplantClientData : BaseModel
    {
        public string Seller { get; set; }
        public string Receiver { get; set; }

        public string ReceiverPass { get; set; }
        public float Price { get; set; }
        public int Index { get; set; }
        public string Description { get; set; }

        public ImplantClientData()
        {
        }

        public ImplantClientData(string seller, string receiver, string receiverpass, float price, int index)
        {
            Seller = seller;
            Receiver = receiver;
            ReceiverPass = receiverpass;
            Price = price;
            Index = index;
            Description = "";
        }

        public override void Validate()
        {
            Try.NotEmpty(Seller, $"Поле {nameof(Seller)} не должно быть пустым");
            Try.NotEmpty(Receiver, $"Поле {nameof(Receiver)} не должно быть пустым");
            Try.NotEmpty(ReceiverPass, $"Поле {nameof(ReceiverPass)} не должно быть пустым");
            Try.Condition(Price >= 0, $"Сумма покупки должна быть неотрицательной");
            Try.Condition(Index >= 0, $"Сумма покупки должна быть неотрицательной");
        }
    }
}