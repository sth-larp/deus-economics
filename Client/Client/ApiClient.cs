using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DeusClient.Client
{
    internal class ApiClient : RestClient
    {
        ILog _log;

        public static X509Certificate[] KnownCertificates { get; private set; }

        public ApiClient(string baseUrl, string logName)
            : base(baseUrl)
        {
            _log = LogManager.GetLogger(logName);
        }

        void FormatJsonContent(IRestResponse response)
        {
            var clearContentType = response.GetClearContentType();

            if (clearContentType == "application/json"
                || clearContentType == "application/javascript"
                || clearContentType == "text/javascript")
            {
                response.Content = JToken.Parse(response.Content).ToString(Newtonsoft.Json.Formatting.Indented);
            }
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            var response = base.Execute(request);

            FormatJsonContent(response);

            _log.Info(RequestString(request));
            _log.Info(ResponseString(response));

            return response;
        }

        public override RestRequestAsyncHandle ExecuteAsync(IRestRequest request,
            Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            var cb2 = new Action<IRestResponse, RestRequestAsyncHandle>((x, y) =>
            {
                FormatJsonContent(x);
                _log.Info(ResponseString(x));
                callback(x, y);
            });

            var ret = base.ExecuteAsync(request, cb2);

            _log.Info(RequestString(request));
            return ret;
        }

        private string RequestString(IRestRequest request)
        {
            var str = new StringBuilder();
            str.Append("API REQUEST ");
            str.Append(BuildUri(request).ToString());
            str.Append(' ');
            str.Append(request.Method);
            str.Append(' ');
            str.AppendLine();

            foreach (var parametr in request.Parameters.Where(x => x.Type == ParameterType.HttpHeader))
                str.AppendLine($"[({parametr.Type}) {parametr.Name}: {parametr.Value}]");
            foreach (var parametr in request.Parameters.Where(x => x.Type == ParameterType.Cookie))
                str.AppendLine($"[({parametr.Type}) {parametr.Name}: {parametr.Value}]");

            str.AppendLine();

            foreach (var parametr in request.Parameters.Where(x => x.Type == ParameterType.UrlSegment))
                str.AppendLine($"({parametr.Type}) {parametr.Name}: {parametr.Value}");
            foreach (var parametr in request.Parameters.Where(x => x.Type == ParameterType.QueryString))
                str.AppendLine($"({parametr.Type}) {parametr.Name}: {parametr.Value}");
            foreach (var parametr in request.Parameters.Where(x => x.Type == ParameterType.GetOrPost))
                str.AppendLine($"({parametr.Type}) {parametr.Name}: {parametr.Value}");

            var body = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody);
            if (body != null)
                str.AppendLine(body.Value.ToString());

            return str.ToString();
        }

        private string ResponseString(IRestResponse response)
        {
            var str = new StringBuilder();
            str.Append("API RESPONSE ");
            if (response.ResponseUri == null)
                str.Append("<empty>");
            else
                str.Append(response.ResponseUri.ToString());
            str.Append(' ');
            str.Append((int)response.StatusCode);
            str.Append(' ');
            str.Append(response.StatusCode);
            str.Append(' ');
            str.AppendLine();
            foreach (var header in response.Headers)
                str.AppendLine($"[{header.Name}: {header.Value}]");
            str.AppendLine();
            str.AppendLine(response.Content);
            return str.ToString();
        }

        static ApiClient()
        {
            LoadCertificates();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback
                += ValidateServerCertificate;
        }

        private static void LoadCertificates()
        {
            var knownCertificateData = new[]
            {
                LoadResource(Assembly.GetExecutingAssembly(), "TestWispCloud.cer"),
            };

            KnownCertificates = knownCertificateData.Select(x =>
            {
                var cert = new X509Certificate();
                cert.Import(x);
                return cert;
            }
            ).ToArray();
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // For debug only
            // return true;

            // Check certificate chain for validity 
            var chainValid = chain.ChainStatus.Length > 0
                && chain.ChainStatus.All(x => x.Status == X509ChainStatusFlags.NoError);
            // or compare with one of predefined certificates in assembly
            var oneOfPredefined = KnownCertificates.Any(x => x.GetCertHashString() == certificate.GetCertHashString());
            return chainValid || oneOfPredefined;
        }

        private static byte[] LoadResource(Assembly assembly, string filename)
        {
            var name = assembly.GetName().Name;
            using (Stream resFilestream = assembly
                .GetManifestResourceStream($"{name}.{filename}"))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }

}
