using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public class SetInsuranceClientData : BaseModel
    {
        public string Company { get; set; }
        public string Holder { get; set; }
        public string Password { get; set; }
        public int? Level { get; set; }

        public SetInsuranceClientData()
        {
            Level = Level ?? 1;
        }

        public override void Validate()
        {
            Try.NotEmpty(Company, $"Поле {nameof(Company)} не должно быть пустым");
            Try.NotEmpty(Holder, $"Поле {nameof(Holder)} не должно быть пустым");
            Try.NotEmpty(Password, $"Поле {nameof(Password)} не должно быть пустым");
        }
    }
}