using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class AccPropertyClientData : BaseModel
    {
        public string Login { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public AccountRole? Role { get; set; }
        public AccountStatus? Status { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, $"{nameof(Login)} cant be empty.");
        }

    }

}