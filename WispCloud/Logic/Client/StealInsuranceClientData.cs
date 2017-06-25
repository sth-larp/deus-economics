using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public class StealInsuranceClientData : BaseModel
    {
        public string Loser { get; set; }
        public string Receiver { get; set; }
        public override void Validate()
        {
            Try.NotEmpty(Loser, $"Sender can't be empty.");
            Try.NotEmpty(Receiver, $"Receiver can't be empty.");
        }
    }
}