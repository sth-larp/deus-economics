using System;
using System.Linq;
using DeusCloud.Data;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.GameEvents;
using DeusCloud.Identity;
using DeusCloud.Logic.Events;
using DeusCloud.Logic.Managers;
using DeusCloud.SignalR;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic
{
    public class UserContext : IDisposable
    {
        public static string DefaultConnectionStringName { get; }
        public static ConnectionMapping SignalRConnectionMapping { get; private set; }

        static UserContext()
        {
            DefaultConnectionStringName = "DeusMaster";
            SignalRConnectionMapping = new ConnectionMapping();
        }

        bool _isDisposed;
        string _dbNameOrConnectionString;

        DeusData _data;
        RightsManager _rights;
        AccountsManager _accounts;
        TransactionsManager _transactions;
        PaymentsManager _payments;
        ConstantManager _constants;
        InsuranceManager _insurances;
        StatManager _stat;

        EventsManager _events;

        public string CurrentAuthorization { get; private set; }
        public Account CurrentUser { get; private set; }

        public DeusData Data
        {
            get
            {
                if (_data == null)
                    _data = new DeusData(_dbNameOrConnectionString);

                return _data;
            }
        }
        public RightsManager Rights
        {
            get
            {
                if (_rights == null)
                    _rights = new RightsManager(this);

                return _rights;
            }
        }

        public StatManager Stat
        {
            get
            {
                if (_stat == null)
                    _stat = new StatManager(this);

                return _stat;
            }
        }

        public ConstantManager Constants
        {
            get
            {
                if (_constants == null)
                {
                    _constants = new ConstantManager(this);
                }

                return _constants;
            }
        }

        public InsuranceManager Insurances
        {
            get
            {
                if (_insurances == null)
                {
                    _insurances = new InsuranceManager(this);
                }

                return _insurances;
            }
        }
        public AccountsManager Accounts
        {
            get
            {
                if (_accounts == null)
                    _accounts = new AccountsManager(this);

                return _accounts;
            }
        }
        public TransactionsManager Transactions
        {
            get
            {
                if (_transactions == null)
                {
                    _transactions = new TransactionsManager(this);
                    _constants = new ConstantManager(this);
                }

                return _transactions;
            }
        }

        public PaymentsManager Payments
        {
            get
            {
                if (_payments == null)
                    _payments = new PaymentsManager(this);

                return _payments;
            }
        }
      
        public UserContext(string dbNameOrConnectionString = null)
        {
            this._dbNameOrConnectionString = string.IsNullOrEmpty(dbNameOrConnectionString)
                ? DefaultConnectionStringName : dbNameOrConnectionString;
        }

        public void AddGameEvent(string user, GameEventType t, string comment, bool isAnonym = false)
        {
            if (!isAnonym)
                comment += $" пользователем {CurrentUser?.Login??"N/A"}";
            var e = new GameEvent(t, user);
            e.Description = comment;
            Data.GameEvents.Add(e);
            Data.SaveChanges();
        }

        ~UserContext()
        {
            if (!_isDisposed)
                Dispose();
        }
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            _data?.Dispose();
        }

        public void SetCurrentUser(Account user)
        {
            CurrentUser = user;
        }

    }

}