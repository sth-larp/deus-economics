using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public class ConstantClientData : BaseModel
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Name, $"Не задано название константы");
        }

    }
}