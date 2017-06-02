using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Accounts.Client;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Accounts
{
    public sealed class AccountsManager : ContextHolder
    {
        UserManager _userManager;

        public AccountsManager(UserContext context)
            : base(context)
        {
            _userManager = new UserManager(UserContext);
        }

        public UserAccount Registration(RegistrationClientData clientData)
        {
            var newUser = new UserAccount(clientData.Email, AccountRole.Admin);
                //register all users as superusers for debug and tests
            newUser.Settings = clientData.Settings;
            newUser.Cash = 100000;

            var result = _userManager.Create(newUser, clientData.Password);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            //var mailBody = TemplateRenderer.GetEmbeddedResource("StaticConstants.Templates.Files.RegistrationEmail.html");
            //var sendTask = _userManager.SendEmailAsync(newUser.Login, "Welcome to Wisp", mailBody);

            return newUser;
        }

        public void ChangePassword(ChangePasswordClientData clientData)
        {
            var account = _userManager.FindById(clientData.Login);
            Try.NotNull(account, $"Cant find account with login: {clientData.Login}.");

            IdentityResult result;
            if (clientData.Login != UserContext.CurrentUser.Login)
            {
                Try.Condition((UserContext.CurrentUser.Role | AccountRole.Admin) > 0,
                    $"Only Administrators can change user account password: {clientData.Login}.");
                result = _userManager.NewPassword(clientData.Login, clientData.NewPassword);
            }
            else
            {
                result = _userManager.ChangePassword(clientData.Login, clientData.CurrentPassword,
                    clientData.NewPassword);
            }

            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());
        }

        public Account Get(string login)
        {
            return _userManager.FindById(login);
        }

        public Account Get(string login, string password)
        {
            return _userManager.Find(login, password);
        }

        public void Update(Account account)
        {
            var result = _userManager.Update(account);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());
        }
    }
}