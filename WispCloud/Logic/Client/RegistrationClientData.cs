using System.Data.Entity;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class RegistrationClientData : BaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Workplace { get; set; }
        public int? SalaryLevel { get; set; }
        public AccountStatus? Status { get; set; }
        public bool? IsAdmin { get; set; }

        public InsuranceType? Insurance;
        public int? InsuranceLevel { get; set; }

        public AccountRole? Role { get; set; }
        public string Alias { get; set; }

        public int? Cash;
        //public UserSettings Settings { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, "Пустое поле Login");
            //Try.NotEmpty(Password, "Пустое поле Password");

            Try.Condition(Cash == null || Cash >= 0, "Поле Cash должно быть неотрицательным");
            Try.Condition(SalaryLevel == null || SalaryLevel >= 0, "Поле SalaryLevel должно быть неотрицательным");
            Try.Condition(SalaryLevel == null || SalaryLevel <= 3, "Поле SalaryLevel должно быть в диапазоне 0 -3");
            Try.Condition(InsuranceLevel == null || InsuranceLevel >= 0, "Поле InsuranceLevel должно быть неотрицательным");
            
        }
    }

}