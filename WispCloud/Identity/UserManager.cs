using System.Threading.Tasks;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Helpers;
using DeusCloud.Logic;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Identity
{
    public sealed class UserManager : UserManager<Account>, IContextHolder
    {
        static PasswordValidator _passwordValidator { get; }

        static UserManager()
        {
            _passwordValidator = new PasswordValidator()
            {
                RequiredLength = 6,
            };
        }

        public UserContext UserContext { get; private set; }

        public UserManager(UserContext userContext)
            : base(new UserStore(userContext))
        {
            UserContext = userContext;
            EmailService = Services.EmailService.Instance;
            PasswordValidator = _passwordValidator;
        }

        public async Task<IdentityResult> NewPasswordAsync(string login, string newPassword)
        {
            var passwordStore = (Store as IUserPasswordStore<Account>);
            if (passwordStore == null)
                return IdentityResult.Failed("Current UserStore doesn't implement IUserPasswordStore");

            var passwordValidateResult = await PasswordValidator.ValidateAsync(newPassword);
            if (!passwordValidateResult.Succeeded)
                return passwordValidateResult;

            var account = await Store.FindByIdAsync(login);
            if (account == null)
                return IdentityResult.Failed($"Cant find account with login: {login}.");

            var newPasswordHash = PasswordHasher.HashPassword(newPassword);
            await passwordStore.SetPasswordHashAsync(account, newPasswordHash);

            return await UpdateAsync(account);
        }

    }

    public static class UserManagerExtensions
    {
        public static IdentityResult NewPassword(this UserManager manager, string login, string newPassword)
        {
            Try.Argument(manager, nameof(manager));
            return AsyncHelper.RunSync(() => manager.NewPasswordAsync(login, newPassword));
        }

    }

}