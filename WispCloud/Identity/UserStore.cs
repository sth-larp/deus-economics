using System;
using System.Threading.Tasks;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Identity
{
    public sealed class UserStore : ContextHolder,
        IUserStore<Account>,
        IUserEmailStore<Account>,
        IUserPasswordStore<Account>
    {
        public UserStore(UserContext context)
            : base(context)
        {
        }

        public void Dispose()
        {
        }

        public async Task CreateAsync(Account user)
        {
            var existingAccount = await FindByIdAsync(user.Login);
            if (existingAccount != null)
                throw new DeusDuplicateException("This login is already in use");

            UserContext.Data.Accounts.Add(user);
            await UserContext.Data.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account user)
        {
            await UserContext.Data.SaveChangesAsync();
        }

        public async Task DeleteAsync(Account user)
        {
            UserContext.Data.Accounts.Remove(user);
            await UserContext.Data.SaveChangesAsync();
        }

        public async Task<Account> FindByIdAsync(string userId)
        {
            return await UserContext.Data.Accounts.FindAsync(userId);
        }

        public async Task<Account> FindByNameAsync(string userName)
        {
            return await FindByIdAsync(userName);
        }

        public Task<string> GetPasswordHashAsync(Account user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(Account user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(Account user, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                user.PasswordHash = null;
                user.TokenSalt = Guid.Empty;
            }
            else
            {
                user.PasswordHash = passwordHash;
                user.TokenSalt = Guid.NewGuid();
            }

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(Account user)
        {
            return Task.FromResult(user.Login);
        }

        public Task SetEmailAsync(Account user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(Account user)
        {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(Account user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public async Task<Account> FindByEmailAsync(string email)
        {
            return await FindByIdAsync(email);
        }

    }

}