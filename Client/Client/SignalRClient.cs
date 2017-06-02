using log4net;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WispCloudClient
{
    public sealed class SignalRClient : IDisposable
    {
        bool _isDisposed;
        ILog _log;

        public HubConnection RawConnection { get; private set; }
        public IHubProxy EventsHubProxy { get; private set; }

        static SignalRClient()
        {
            ServicePointManager.DefaultConnectionLimit = 10;
        }

        //http://stackoverflow.com/questions/18074912/signalr-difference-between-on-and-subscribe-of-ihubproxy

        public SignalRClient(string host, bool enableLogs = false, string logName = null)
        {
            if (logName == null)
                logName = CloudClient.DefaultLogName;

            RawConnection = new HubConnection($"{host}/signalr");

            if (enableLogs)
            {
                _log = LogManager.GetLogger(logName);
                RawConnection.Received += Received;
            }

            EventsHubProxy = RawConnection.CreateHubProxy("EventsHub");

            //var subscription = EventsHubProxy.Subscribe("PlanChange");
            //subscription.Received += OnReceived;
        }


        ~SignalRClient()
        {
            if (!_isDisposed)
                Dispose();
        }
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            RawConnection?.Dispose();
        }

        void Received(string obj)
        {
            _log.Info($"SIGNALR PUSH \n{obj}");
        }

        public void Authenticate(Authenticator authenticator)
        {
            authenticator.Authenticate(RawConnection);
        }

        public async Task Start()
        {
            await RawConnection.Start();
        }

        public void Stop()
        {
            RawConnection.Stop();
        }

        public async Task Restart()
        {
            Stop();
            await Start();
        }

    }

}
