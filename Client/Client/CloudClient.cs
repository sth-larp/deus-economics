using System;
using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts;
using DeusClient.ApiCommands.Accounts.Client;
using RestSharp;

namespace DeusClient.Client
{
    public sealed class CloudClient : IDisposable
    {
        public static string DefaultLogName { get; }
        public static ApiSerializer ApiSerializer { get; }

        static CloudClient()
        {
            DefaultLogName = "NET Wisp Cloud Client Log";
            ApiSerializer = new ApiSerializer();
        }

        bool _isDisposed;
        RestClient _apiClient;
        SignalRClient _signalRClient;

        LoginCommand _loginCommand;
        RegistrationCommand _registrationCommand;
        GetCurrentAccountCommand _getCurrentAccountCommand;

        public string LogName { get; private set; }
        public string Host { get; private set; }
        public Authenticator Authenticator { get; private set; }
        public Account Account { get; private set; }

        public SignalRClient SignalRClient { get { return _signalRClient; } }

        public CloudClient(string host, bool enableLogs = false, string logName = null)
        {
            if (logName == null)
                logName = DefaultLogName;

            this.LogName = logName;
            this.Host = host;

            this._apiClient = (enableLogs ? new ApiClient(this.Host, logName) : new RestClient(this.Host));
            this._apiClient.ClearHandlers();
            this._apiClient.AddHandler("application/json", ApiSerializer);
            this._apiClient.AddHandler("text/json", ApiSerializer);

            this._signalRClient = new SignalRClient(this.Host, enableLogs, logName);

            this._loginCommand = new LoginCommand();
            this._registrationCommand = new RegistrationCommand();
            this._getCurrentAccountCommand = new GetCurrentAccountCommand();
        }
        ~CloudClient()
        {
            if (!_isDisposed)
                Dispose();
        }
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            _signalRClient?.Dispose();

        }

        public RestRequest CreateRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.JsonSerializer = ApiSerializer;
            return request;
        }

        public async Task<IRestResponse<ResponseBodyType>> ExecuteAsync<ResponseBodyType>(RestRequest request)
        {
            var responseTask = _apiClient.ExecuteTaskAsync<ResponseBodyType>(request);
            await responseTask;
            return responseTask.Result;
        }

        public async Task<IRestResponse> ExecuteAsync(RestRequest request)
        {
            var requestTask = _apiClient.ExecuteTaskAsync(request);
            await requestTask;
            return requestTask.Result;
        }

        async Task<Account> GetCurrentAccount()
        {
            var accountTask = _getCurrentAccountCommand.ExecuteAsync(this);
            await accountTask;

            if (!accountTask.IsCompleted)
                return null;

            if (!accountTask.Result.IsSuccessful)
                return null;

            return accountTask.Result.Content;
        }

        async Task<Authorization> LoginCoreAsync(string login, string password)
        {
            var loginTask = _loginCommand.ExecuteAsync(this, login, password);
            await loginTask;

            if (!loginTask.IsCompleted)
                return null;

            if (!loginTask.Result.IsSuccessful)
                return null;

            return loginTask.Result.Content;
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            var getLoginCoreTask = LoginCoreAsync(login, password);
            await getLoginCoreTask;
            if (getLoginCoreTask.Result == null)
                return false;

            Authenticator = new Authenticator(getLoginCoreTask.Result);
            _apiClient.Authenticator = Authenticator;

            var getAccountTask = GetCurrentAccount();
            await getAccountTask;
            if (getAccountTask.Result == null)
            {
                _apiClient.Authenticator = null;
                return false;
            }

            _signalRClient.Stop();
            if (Authenticator != null)
            {
                _signalRClient.Authenticate(Authenticator);
                await _signalRClient.Start();
            }

            Account = getAccountTask.Result;

            return true;
        }

        public async Task<bool> RegistrationAsync(RegistrationClientData clientData)
        {
            var registrationTask = _registrationCommand.ExecuteAsync(this, clientData);
            await registrationTask;

            if (!registrationTask.IsCompleted)
                return false;

            if (!registrationTask.Result.IsSuccessful)
                return false;

            var loginTask = LoginAsync(clientData.Email, clientData.Password);
            await loginTask;

            if (!loginTask.IsCompleted)
                return false;

            return loginTask.Result;
        }

    }

}
