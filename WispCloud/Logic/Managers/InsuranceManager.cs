using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.GameEvents;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Server;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class InsuranceManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        private static string CorpName1 = "JJ";
        private static string CorpName2 = "Serenity";
        private static string CorpName3 = "Panam";

        private static Dictionary<string, InsuranceType> _associations = 
            new Dictionary<string, InsuranceType>
        {
            {"govt", InsuranceType.Govt },
            {CorpName1, InsuranceType.JJ},
            {CorpName2, InsuranceType.Serenity},
            {CorpName3, InsuranceType.Panam},
            {"admin", InsuranceType.SuperVip}
        }; 

        public InsuranceManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
        }

        public List<Loyalty> GetLoyalties()
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            return UserContext.Data.Loyalties.ToList();
        }

        public void DeleteLoyalty(int id)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loyalty = UserContext.Data.Loyalties.Find(id);
            Try.NotNull(loyalty, $"Can't find insurance loyalty relation with id: {id}");

            UserContext.Data.Loyalties.Remove(loyalty);
            UserContext.Data.SaveChanges();

            UserContext.AddGameEvent(loyalty.LoyalName, GameEventType.Insurance, 
                $"{loyalty.LoyalService.Login} перестал обслуживать страховку {loyalty.LoyalName}", true);

            var corp = _userManager.FindById(loyalty.LoyalName);
            if (corp == null) return;

            UserContext.AddGameEvent(corp.Login, GameEventType.Insurance,
                $"{loyalty.LoyalService.Login} перестал обслуживать страховку {loyalty.LoyalName}");
        }

        public Loyalty NewLoyalty(Loyalty data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var corp = _userManager.FindById(data.LoyalName);
            Try.NotNull(corp, $"Не найден логин: {corp}");
            Try.Condition((corp.Role & AccountRole.Company) > 0, 
                $"{corp} не является организацией типа {AccountRole.Company}");

            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == corp.Login && x.Insurance == data.Insurance);
            Try.Condition(!check.Any(), $"Компания уже обслуживает данную страховку");
            Try.Condition(data.MinLevel < 4 && data.MinLevel > 0, $"Значения MinLevel разрешены в диапазоне [1:3]");

            var loyalty = new Loyalty(corp, data.Insurance);
            loyalty.MinLevel = data.MinLevel;

            UserContext.Data.Loyalties.Add(loyalty);
            UserContext.Data.SaveChanges();

            UserContext.AddGameEvent(loyalty.LoyalService.Login, GameEventType.Insurance,
                $"{loyalty.LoyalService.Login} начал обслуживать страховку {loyalty.LoyalName}", true);

            UserContext.AddGameEvent(corp.Login, GameEventType.Insurance,
                $"{loyalty.LoyalService.Login} начал обслуживать страховку {loyalty.LoyalName}");

            return loyalty;
        }

        public int CheckLoyaltyLevel(Account person, Account service)
        {
            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == service.Login && x.Insurance == person.Insurance);
            if (check.Any())
                return person.EffectiveLevel;
            return 0;
        }

        public List<Loyalty> GetCompanyLoyalties(string login)
        {
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);

            return UserContext.Data.Loyalties.Where(x => x.LoyalService.Login == login).ToList();
        }

        public List<InsuranceHolderServerData> GetInsuranceHolders(string login)
        {
            var ret = new List<InsuranceHolderServerData>();
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            Try.Condition(_associations.ContainsKey(login), $"{login} не выпускает страховки");
            var t = _associations[login];
            var list = UserContext.Data.Accounts.Where(x => x.Insurance == t);

            foreach (var user in list.Where(x => !x.InsuranceHidden))
            {
                var h = new InsuranceHolderServerData(user.Login, login)
                {
                    InsuranceLevel = user.InsuranceLevel,
                    Insurance = t,
                    UserFullname = user.Fullname??""
                };
                ret.Add(h);
            }
            return ret;
        }

        public void RemoveInsuranceHolder(string user)
        {
            var userAccount = _userManager.FindById(user);
            Try.NotNull(userAccount, $"Не найден пользователь {user}");
            Try.Condition(userAccount.Insurance != InsuranceType.None, $"У пользователя {user} нет страховки");

            var corpAccount = GetIssuerFromType(userAccount.Insurance);
            _rightsManager.CheckForAccessOverSlave(corpAccount, AccountAccessRoles.Withdraw);
            
            RemoveInsuranceHolder_Checked(userAccount, corpAccount.Login);
        }

        private void RemoveInsuranceHolder_Checked(Account userAccount, string company)
        {
            userAccount.Insurance = InsuranceType.None;
            userAccount.InsuranceLevel = 1;
            UserContext.Accounts.Update(userAccount);

            UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance,
                $"{company} отменила вашу страховку", true);

            UserContext.AddGameEvent(company, GameEventType.Insurance,
                $"отменена страховка {userAccount.Login}");
        }

        public void SetInsuranceHolder(SetInsuranceClientData data)
        {
            _rightsManager.CheckForAccessOverSlave(data.Company, AccountAccessRoles.Withdraw);

            var userAccount = _userManager.FindById(data.Holder);
            var pass = _userManager.CheckPassword(userAccount, data.Password);
            var level = data.Level ?? 1;

            Try.Condition(pass, $"Неверный пароль пользователя {data.Holder}");
            Try.Condition(_associations.ContainsKey(data.Company), 
                $"{data.Company} не выпускает страховки");

            var t = _associations[data.Company];
            Try.Condition(CheckInsuranceLevel(level, t), $"Неверный уровень страховки {level}");

            SetInsuranceHolder_Checked(userAccount, level, t);
        }

        private void SetInsuranceHolder_Checked(Account userAccount, int level, InsuranceType t, bool isStolen = false)
        {
            Try.Condition((userAccount.Role & AccountRole.Person) > 0,
                $"Только персоны могут быть держателями страховки");

            var oldIssuer = GetIssuerFromType(userAccount.Insurance);
            var newIssuer = GetIssuerFromType(t);

            if (!isStolen)
            {
                var price = level;
                if (userAccount.Insurance == t)
                    price = Math.Max(0, level - userAccount.InsuranceLevel);

                Try.Condition(newIssuer.InsurancePoints >= price, $"Не хватает очков страховки");
                newIssuer.InsurancePoints -= price;
                UserContext.Accounts.Update(newIssuer);

                var txtverb = userAccount.Insurance == t ? "изменение" : "выдачу";
                UserContext.AddGameEvent(newIssuer.Login, GameEventType.Index,
                    $"Потрачено {price} очков на {txtverb} страховки {userAccount.Login}", true);
            }

            //Отменить старую страховку, если была
            if (oldIssuer != null && userAccount.Insurance != t)
            {
                UserContext.AddGameEvent(oldIssuer.Login, GameEventType.Insurance,
                    $"{userAccount.Login} отказался от вашей страховки", true);

                UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance,
                    $"Вы отказались от страховки {oldIssuer.Login}", true);
            }

            userAccount.Insurance = t;
            userAccount.InsuranceLevel = level;
            UserContext.Accounts.Update(userAccount);

            if(!isStolen)
                UserContext.AddGameEvent(newIssuer.Login, GameEventType.Insurance,
                    $"Выдана страховка {userAccount.Login} " +
                    $"уровня {userAccount.InsuranceLevel}");

            UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance,
                $"Выдана страховка {userAccount.Insurance} " +
                $"уровня {userAccount.InsuranceLevel}", true);
        }

        public void StealInsurance(StealInsuranceClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loserAccount = _userManager.FindById(data.Loser);
            Try.NotNull(loserAccount, $"Не найден пользователь {data.Loser}");

            Try.Condition(loserAccount.Insurance != InsuranceType.None, 
                $"У пользователя {data.Loser} нет страховки");

            var receiverAccount = _userManager.FindById(data.Receiver);
            Try.NotNull(receiverAccount, $"Не найден пользователь {data.Receiver}");

            receiverAccount.InsuranceHidden = true;
            SetInsuranceHolder_Checked(receiverAccount, loserAccount.InsuranceLevel, loserAccount.Insurance, true);
            RemoveInsuranceHolder(data.Loser);
        }

        private bool CheckInsuranceLevel(int level, InsuranceType t)
        {
            if (level <= 0) return false;
            if (IsCorporate(t) && level <= 3) return true;
            if(t == InsuranceType.Govt && level <= 2) return true;
            return level == 1;
        }

        private bool IsCorporate(InsuranceType insurance)
        {
            return insurance == InsuranceType.Panam
                   || insurance == InsuranceType.JJ
                   || insurance == InsuranceType.Serenity;
        }

        private Account GetIssuerFromType(InsuranceType t)
        {
            var pair = _associations.FirstOrDefault(x => x.Value == t);
            if (pair.Key == null) return null;
            return _userManager.FindById(pair.Key);
        }

        public void SwitchCycle(SwitchCycleClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            ResetIndexValues(data);
            RemoveStolenInsurances();
            ProlongInsurance();
        }

        private void RemoveStolenInsurances()
        {
            var holders = UserContext.Data.Accounts.Where(x => x.InsuranceHidden).ToList();
            foreach (var holder in holders)
            {
                UserContext.AddGameEvent(holder.Login, GameEventType.Index,
                    $"{holder.Insurance} отменила вашу страховку", true);
                holder.Insurance = InsuranceType.None;
                holder.InsuranceLevel = 1;
                holder.InsuranceHidden = false;
                UserContext.Accounts.Update(holder);
            }
        }

        private void ProlongInsurance()
        {
            foreach (var kv in _associations)
            {
                var corp = _userManager.FindById(kv.Key);
                if (corp == null) continue; //Not found in DB

                var holders = GetInsuranceHolders(kv.Key).OrderByDescending(x => x.InsuranceLevel).ToList();
                foreach (var holder in holders)
                {
                    if (corp.InsurancePoints >= holder.InsuranceLevel)
                    {
                        corp.InsurancePoints -= holder.InsuranceLevel;
                        UserContext.AddGameEvent(corp.Login, GameEventType.Index, 
                            $"Продлена страховка пользователя {holder.UserLogin}," +
                            $" потрачено {holder.InsuranceLevel} очков", true);
                        UserContext.AddGameEvent(holder.UserLogin, GameEventType.Index,
                            $"{corp.Login} продлила вашу страховку", true);
                    }
                    else
                    {
                        var userAccount = _userManager.FindById(holder.UserLogin);

                        RemoveInsuranceHolder_Checked(userAccount, corp.Login);
                    }
                }
                UserContext.Accounts.Update(corp);
            }
        }
        private void ResetIndexValues(SwitchCycleClientData data)
        {
            foreach (var pair in _associations)
            {
                var corp = _userManager.FindById(pair.Key);
                if (corp == null) continue; //Not found in DB

                var cIndex = data.Indexes.ToList().FindIndex(x => x.Key == pair.Key);
                if (cIndex >= 0)
                    corp.Index = data.Indexes[cIndex].Value;
                corp.InsurancePoints = corp.Index;
                UserContext.Accounts.Update(corp);

                UserContext.AddGameEvent(corp.Login, GameEventType.Index,
                    $"Выставлен индекс и очки страховки {corp.Index}");
            }
        }       
    }
}