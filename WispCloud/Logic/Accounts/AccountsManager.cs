using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.Accounts.Client;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;
using WispCloud;
using WispCloud.Identity;
using WispCloud.Templates;

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
            var newUser = new UserAccount(clientData.Email, AccountRole.Master); //register all users as superusers for debug and tests
            newUser.Settings = clientData.Settings;

            var result = _userManager.Create(newUser, clientData.Password);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            //var mailBody = TemplateRenderer.GetEmbeddedResource("WispCloud.Templates.Files.RegistrationEmail.html");
            //var sendTask = _userManager.SendEmailAsync(newUser.Login, "Welcome to Wisp", mailBody);

            return newUser;
        }

        public void RestoreUserPassword(string email)
        {
            var account = _userManager.FindById(email);
            Try.NotNull(account, $"Cant find account with email: {email}.");
            Try.Condition(account.Active, $"Cant restore password for not active user.");
            Try.Condition(account.Role != AccountRole.Hub, $"Cant restore password for hub.");

            var newPassword = NewPassword(account.Login);

            var mailBody = TemplateRenderer.RenderTemplate(
                    "WispCloud.Templates.Files.RestorePasswordEmail.cshtml",
                    new RestorePasswordModel() { NewPassword = newPassword });
            var sendTask = _userManager.SendEmailAsync(account.Login, "Restore Wisp password", mailBody);
        }

        public void ChangePassword(ChangePasswordClientData clientData)
        {
            var account = _userManager.FindById(clientData.Login);
            Try.NotNull(account, $"Cant find account with login: {clientData.Login}.");

            var result = _userManager.ChangePassword(clientData.Login, clientData.CurrentPassword, clientData.NewPassword);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());
        }

        public string NewPassword(string login)
        {
            var newPassword = StaticRandom.GenerateString(8);

            var result = _userManager.NewPassword(login, newPassword);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            return newPassword;
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