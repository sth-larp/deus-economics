using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class ChangePasswordClientData : BaseModel
    {
        public string Login { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, $"Login cant be empty.");
            Try.NotEmpty(CurrentPassword, $"CurrentPassword cant be empty.");
            Try.NotEmpty(NewPassword, $"NewPassword cant be empty.");
        }
    }
}