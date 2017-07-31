using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Alice;
using DeusCloud.Data.Entities.GameEvents;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Helpers;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Server;
using DeusCloud.Serialization;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class InsuranceManager : ContextHolder
    {
        private RightsManager _rightsManager;

        private static Dictionary<string, InsuranceType> _associations = 
            new Dictionary<string, InsuranceType>
        {
            {"Govt", InsuranceType.Govt },
            {"JJ", InsuranceType.JJ},
            {"Serenity", InsuranceType.Serenity},
            {"Panam", InsuranceType.Panam},
            {"admin", InsuranceType.SuperVip}
        }; 

        public InsuranceManager(UserContext context) : base(context)
        {
            _rightsManager = new RightsManager(UserContext);
        }

        public List<LoyaltyServerData> GetLoyalties()
        {
            var list = UserContext.Data.Loyalties.ToList();
            var newList = new List<LoyaltyServerData>();

            list.ForEach(x =>
            {
                var l = new LoyaltyServerData(x.Id, x.Insurance, x.LoyalService.Login, x.LoyalService.Fullname);
                l.MinLevel = x.MinLevel;
                newList.Add(l);
            });
            return newList;
        }

        public void DeleteLoyalty(int id)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loyalty = UserContext.Data.Loyalties.Find(id);
            Try.NotNull(loyalty, $"Can't find insurance loyalty relation with id: {id}");

            UserContext.AddGameEvent(loyalty.LoyalName, GameEventType.Insurance,
                $"Вы перестали обслуживать страховку {loyalty.Insurance}", true);

            var issuerAcc = GetIssuerFromType(loyalty.Insurance);
            UserContext.AddGameEvent(issuerAcc.Login, GameEventType.Insurance,
                 $"{loyalty.LoyalService.DisplayName} больше не обслуживает вашу страховку", true);

            UserContext.Data.Loyalties.Remove(loyalty);
            UserContext.Data.SaveChanges();
        }

        public Loyalty NewLoyalty(Loyalty data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loyalAcc = UserContext.Accounts.GetOrFail(data.LoyalName);//_userManager.FindById(data.LoyalName);

            Try.Condition(loyalAcc.Role.IsCompany(), $"{loyalAcc.Login} не является компанией");

            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == loyalAcc.Login && x.Insurance == data.Insurance);
            Try.Condition(!check.Any(), $"Компания уже обслуживает данную страховку");
            Try.Condition(data.MinLevel < 4 && data.MinLevel > 0, $"Значения MinLevel разрешены в диапазоне [1:3]");

            var loyalty = new Loyalty(loyalAcc, data.Insurance);
            loyalty.MinLevel = data.MinLevel;

            UserContext.Data.Loyalties.Add(loyalty);
            UserContext.Data.SaveChanges();

            var issuerAcc = GetIssuerFromType(data.Insurance);

            UserContext.AddGameEvent(issuerAcc.Login, GameEventType.Insurance,
                $"Вашу страховку теперь обслуживает {loyalAcc.DisplayName}", true);

            UserContext.AddGameEvent(loyalAcc.Login, GameEventType.Insurance,
                $"Вы начали обслуживать страховку {loyalty.Insurance}", true);

            return loyalty;
        }

        public int CheckInsuranceGrade(Account person, Account service)
        {
            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == service.Login && x.Insurance == person.Insurance);

            if (check.Any())
                return person.EffectiveLevel;
            return 0;
        }

        public List<Loyalty> GetCompanyLoyalties(string login)
        {
            var acc = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);

            return UserContext.Data.Loyalties.Where(x => x.LoyalService.Login == acc.Login).ToList();
        }

        public List<InsuranceHolderServerData> GetInsuranceHolders(string login)
        {
            var ret = new List<InsuranceHolderServerData>();
            var acc = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            Try.Condition(_associations.ContainsKey(acc.Login), $"{acc.Login} не выпускает страховки");

            var t = _associations[acc.Login];
            var list = UserContext.Data.Accounts.Where(x => x.Insurance == t);

            foreach (var user in list.Where(x => !x.InsuranceHidden))
            {
                var h = new InsuranceHolderServerData(user.Login, acc.Login)
                {
                    InsuranceLevel = user.InsuranceLevel,
                    Insurance = t,
                    UserFullname = user.Fullname??""
                };
                ret.Add(h);
            }
            return ret.OrderBy(x => x.UserFullname).ToList();
        }

        public void RemoveInsuranceHolder(string user)
        {
            var userAccount = UserContext.Accounts.GetOrFail(user); 
            Try.Condition(userAccount.Insurance != InsuranceType.None, $"У пользователя {user} нет страховки");

            var corpAccount = GetIssuerFromType(userAccount.Insurance);
            _rightsManager.CheckForAccessOverSlave(corpAccount, AccountAccessRoles.Withdraw);
            
            RemoveInsuranceHolder_Checked(userAccount, corpAccount);
        }

        private void RemoveInsuranceHolder_Checked(Account userAccount, Account companyAccount)
        {
            userAccount.Insurance = InsuranceType.None;
            userAccount.InsuranceLevel = 1;
            UserContext.Accounts.Update(userAccount);

            UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance, 
                $"{companyAccount.DisplayName} отменила вашу страховку", true);

            UserContext.AddGameEvent(companyAccount.Login, GameEventType.Insurance,
                $"Отменена страховка {userAccount.DisplayName}", true);

            UpdateInsuranceInAlice(userAccount);
        }

        public void SetInsuranceHolder(SetInsuranceClientData data)
        {
            var companyAcc = _rightsManager.CheckForAccessOverSlave(data.Company, AccountAccessRoles.Withdraw);

            var userAccount = UserContext.Accounts.GetOrFail(data.Holder, data.Password);

            Try.Condition(_associations.ContainsKey(companyAcc.Login), $"{companyAcc.Login} не выпускает страховки");
            var t = _associations[companyAcc.Login];

            var level = data.Level ?? 0; //t.SetLevel()
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
                    $"Потрачено {price} очков на {txtverb} страховки {userAccount.DisplayName}", true);
            }

            //Отменить старую страховку, если была
            if (oldIssuer != null && userAccount.Insurance != t)
            {
                UserContext.AddGameEvent(oldIssuer.Login, GameEventType.Insurance,
                    $"{userAccount.DisplayName} отказался от вашей страховки", true);

                UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance,
                    $"Вы отказались от страховки {t}", true);
            }

            userAccount.Insurance = t;
            userAccount.InsuranceLevel = level;
            UserContext.Accounts.Update(userAccount);

            if(!isStolen)
                UserContext.AddGameEvent(newIssuer.Login, GameEventType.Insurance,
                    $"Выдана страховка {userAccount.DisplayName}" +
                    $"уровня {userAccount.InsuranceLevel}");

            UserContext.AddGameEvent(userAccount.Login, GameEventType.Insurance,
                $"Выдана страховка {userAccount.Insurance} " +
                $"уровня {userAccount.InsuranceLevel}", true);

            UpdateInsuranceInAlice(userAccount);
        }

        public void StealInsurance(StealInsuranceClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loserAccount = UserContext.Accounts.GetOrFail(data.Loser);
            var receiverAccount = UserContext.Accounts.GetOrFail(data.Receiver);

            Try.Condition(receiverAccount.EffectiveLevel <= loserAccount.EffectiveLevel, 
                "Ваша страховка лучше, чем у жертвы");

            Try.Condition(loserAccount.Insurance != InsuranceType.None, 
                $"У пользователя {loserAccount.Login} нет страховки");

            receiverAccount.InsuranceHidden = true;
            SetInsuranceHolder_Checked(receiverAccount, loserAccount.InsuranceLevel, loserAccount.Insurance, true);
            RemoveInsuranceHolder(loserAccount.Login);
        }

        public void UpdateInsuranceInAlice(Account acc)
        {
            var url = AppSettings.Url("AliceUrl");
            var client = new HttpClient();

            var data = new AliceInsurance(acc);
            var body = WispJsonSerializer.SerializeToJsonString(data);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "ZWNvbm9taWNzOno2dHJFKnJVRHJ1OA==");

            client.SendAsync(request);
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
            return UserContext.Accounts.GetOrFail(pair.Key);
        }

        public void SwitchCycle(SwitchCycleClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                ResetIndexValues(data);
                PayInsuranceMoney();
                RemoveStolenInsurances();
                ProlongInsurance();

                UserContext.Data.SaveChanges();
                dbTransact.Commit();
            }
        }

        private void PayInsuranceMoney()
        {
            foreach (var kv in _associations)
            {
                var issuerAcc = UserContext.Accounts.Get(kv.Key);
                if (issuerAcc == null) continue; //Corp not found in DB

                var holderAccs = UserContext.Data.Accounts.Where(x => x.Insurance == kv.Value).ToList(); 
                foreach (var holderAcc in holderAccs)
                {
                    var salary = UserContext.Constants.GetInsuranceSalary(holderAcc.Insurance, holderAcc.InsuranceLevel);
                    var payment = new Payment(issuerAcc, holderAcc, salary);
                    payment.Employer = issuerAcc.Login;
                    UserContext.Payments.PerformPayment(payment);
                }
            }
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
                var corp = UserContext.Accounts.Get(kv.Key);
                if (corp == null) continue; //Corp not found in DB

                var totalPrice = 0;
                var holders = GetInsuranceHolders(kv.Key).OrderByDescending(x => x.InsuranceLevel).ToList();
                foreach (var holder in holders)
                {
                    var effectivePrice = UserContext.Constants.GetInsuranceCost(holder.Insurance, holder.InsuranceLevel);
                    if (corp.InsurancePoints >= effectivePrice)
                    {
                        totalPrice += effectivePrice;
                        corp.InsurancePoints -= effectivePrice;
                        
                        UserContext.AddGameEvent(holder.UserLogin, GameEventType.Index,
                            $"Ваша страховка {holder.Insurance} продлена", true);
                    }
                    else
                    {
                        var userAccount = UserContext.Accounts.GetOrFail(holder.UserLogin); 

                        RemoveInsuranceHolder_Checked(userAccount, corp);
                    }
                }
                UserContext.AddGameEvent(corp.Login, GameEventType.Index,
                    $"Продлены страховки, потрачено {totalPrice} очков", true);
                //UserContext.Accounts.Update(corp);
            }
        }
        private void ResetIndexValues(SwitchCycleClientData data)
        {
            foreach (var pair in _associations)
            {
                var corp = UserContext.Accounts.Get(pair.Key); 
                if (corp == null) continue; //Not found in DB

                var cIndex = data.Indexes.ToList().FindIndex(x => x.Key == pair.Key);
                if (cIndex >= 0)
                    corp.Index = data.Indexes[cIndex].Value;
                corp.InsurancePoints = corp.Index;
                UserContext.Accounts.Update(corp);

                UserContext.AddGameEvent(corp.Login, GameEventType.Index,
                    $"Выставлен индекс {corp.Index} и очки страховки {corp.InsurancePoints}");
            }
        }       
    }
}