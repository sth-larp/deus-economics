using DeusCloud.Data.Entities;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Events.Client;
using DeusCloud.Logic.Rights;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Transactions
{
    public sealed class TransactionsManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        public TransactionsManager(UserContext context): base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
        }

        public void Transfer(string sender, string receiver, float amount)
        {
            _rightsManager.CheckCurrentUserActive();

            var receiverAcc = _userManager.FindById(receiver);
            Try.NotNull(receiverAcc, $"Cant find account with login: {receiver}.");

            var senderAcc = _userManager.FindById(sender);
            Try.NotNull(senderAcc, $"Cant find account with login: {sender}.");

            using (var transaction = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                _rightsManager.CheckForAccessOverSlave(sender, AccountAccessRoles.Withdraw);

                Try.Condition(senderAcc.Cash >= amount, $"Not enought money");
                Try.Condition(amount > 0, $"Can't transfer negative or zero funds");

                senderAcc.Cash -= amount;
                receiverAcc.Cash += amount;

                UserContext.Data.SaveChanges();
                transaction.Commit();
            }
        }
    }
}