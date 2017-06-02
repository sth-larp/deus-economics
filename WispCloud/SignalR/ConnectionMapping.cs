using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using Microsoft.AspNet.SignalR.Hubs;

namespace DeusCloud.SignalR
{
    public class ConnectionMapping
    {
        readonly ReaderWriterLockSlim _lock;
        /// <summary>
        /// Карта соединений SignalR:
        /// Логин: Токен авторизации: ИД соединения
        /// Map[login][authToken] = connectionUid
        /// </summary>
        SortedDictionary<string, SortedDictionary<string, List<string>>> Map { get; set; }

        public ConnectionMapping()
        {
            this._lock = new ReaderWriterLockSlim();
            this.Map = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
        }

        string GetAuthorization(HubCallerContext context)
        {
            string authorization = context.Request.GetAuthorization();
            Try.NotEmpty(authorization, HttpStatusCode.Unauthorized);

            return authorization;
        }

        string GetLogin(HubCallerContext context)
        {
            Try.Condition(context.User.Identity.IsAuthenticated, HttpStatusCode.Unauthorized);
            return context.User.Identity.Name;
        }

        public void UpdateConnection(HubCallerContext context)
        {
            var authorization = GetAuthorization(context);
            var login = GetLogin(context);
            var connectionID = context.ConnectionId;

            _lock.EnterWriteLock();
            try
            {
                var tokens = GetOrCreateLoginMap(login);
                var tokenMap = GetOrCreateTokenMap(tokens, authorization);
                TryAddConnection(tokenMap, connectionID);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void TryAddConnection(List<string> tokenMap, string connectionID)
        {
            if (!tokenMap.Contains(connectionID))
                tokenMap.Add(connectionID);
        }

        private void TryRemoveConnection(List<string> tokenMap, string connectionID)
        {
            tokenMap.Remove(connectionID);
        }

        private List<string> GetOrCreateTokenMap(SortedDictionary<string, List<string>> tokens, string authorization)
        {
            if (tokens.ContainsKey(authorization))
                return tokens[authorization];

            var tokenMap = new List<string>();
            tokens.Add(authorization, tokenMap);
            return tokenMap;
        }

        private SortedDictionary<string, List<string>> GetOrCreateLoginMap(string login)
        {
            if (Map.ContainsKey(login))
                return Map[login];

            var loginMap = new SortedDictionary<string, List<string>>();
            Map.Add(login, loginMap);
            return loginMap;
        }

        public void RemoveConnection(HubCallerContext context)
        {
            var authorization = GetAuthorization(context);
            var login = GetLogin(context);
            
            _lock.EnterWriteLock();
            try
            {
                var tokens = GetOrCreateLoginMap(login);
                var tokenMap = GetOrCreateTokenMap(tokens, authorization);
                TryRemoveConnection(tokenMap, authorization);

                if (!tokenMap.Any())
                    tokens.Remove(authorization);
                if (!tokens.Any())
                    Map.Remove(login);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public List<string> GetAllConnections()
        {
            var result = Map.Values.SelectMany(x => x.Values.SelectMany(y => y));
            return result.ToList();
        }

        public List<string> GetConnectionIDsForLoginExceptAuthorization(
            string login, string exceptAuthorization)
        {
            if (string.IsNullOrEmpty(login))
                throw new DeusException("login can't be empty", false);

            return GetConnectionIDsForLoginsExceptAuthorization(
                Enumerable.Repeat(login, 1), exceptAuthorization);
        }

        public List<string> GetConnectionIDsForLoginsExceptAuthorization(
            IEnumerable<string> logins, string exceptAuthorization)
        {
            Try.NotNull(logins, "logins list cannot be empty", false);
            _lock.EnterReadLock();
            try
            {
                var loginsArray = logins.ToArray();
                var result = new List<string>();
                result.AddRange(Map.Where(l =>
                    loginsArray.Contains(l.Key)).SelectMany(lmap =>
                        lmap.Value.Where(tokenMap => tokenMap.Key != exceptAuthorization)
                        .SelectMany(x => x.Value)
                    ));
                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

    }

}