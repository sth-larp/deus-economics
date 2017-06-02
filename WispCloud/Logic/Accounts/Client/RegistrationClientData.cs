using System.Net.Mail;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Accounts.Client
{
    public sealed class RegistrationClientData : BaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public UserSettings Settings { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, "Login is empty;");
            Try.NotEmpty(Password, "Password is empty;");

            //check email by .net classes instead of huge regexp,
            //this can pass some strange emails, but it simple to use
            //try
            //{
            //    var addr = new MailAddress(Login);
            //}
            //catch
            //{
            //    throw new DeusException("Incorrect email.");
            //}
        }
    }

}