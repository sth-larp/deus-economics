﻿using System;
using DeusCloud.Data;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Logic.Accounts;
using DeusCloud.Logic.Events;
using DeusCloud.Logic.Transactions;
using WispCloud.Data;
using WispCloud.Logic;
using WispCloud.SignalR;

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
                    _transactions = new TransactionsManager(this);

                return _transactions;
            }
        }
        
        public EventsManager Events
        {
            get
            {
                if (_events == null)
                    _events = new EventsManager(this);

                return _events;
            }
        }
        
       
        public UserContext(string dbNameOrConnectionString = null)
        {
            this._dbNameOrConnectionString = string.IsNullOrEmpty(dbNameOrConnectionString)
                ? DefaultConnectionStringName : dbNameOrConnectionString;
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

        public void SetCurrentUser(Account user, string authorization)
        {
            CurrentUser = user;
            CurrentAuthorization = authorization; //Previously was 'null' - bug?
        }

    }

}