using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class AccIndexClientData : BaseModel
    {
        public string Login { get; set; }
        public int Index { get; set; }
        public int InsurancePoints { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, $"{nameof(Login)} cant be empty.");
            Try.Condition(Index >= 0, $"Index should be positive");
            Try.Condition(InsurancePoints >= 0, $"InsurancePoints should be positive");
        }

    }

}