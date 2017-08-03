using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;

namespace Rolillness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendEventsToAlice(15776);
        }

        public void SendEventsToAlice(int id)
        {
            var url = "https://alice.digital:6984" + "/events";
            var client = new HttpClient();

            var data = new AliceInsurance(id.ToString());
            var body = MyJsonSerializer.SerializeToJsonString(data);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "ZWNvbm9taWNzOno2dHJFKnJVRHJ1OA==");

            client.SendAsync(request);
        }
        public class AliceInsurance
        {
            public string eventType { get; set; }

            public long timestamp { get; set; }

            public string characterId { get; set; }

            public AliceInsuranceData data { get; set; }

            public AliceInsurance(string id)
            {
                eventType = "change-insurance";
                timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);//Ticks;
                characterId = id;
                data = new AliceInsuranceData();
            }
        }

        public class AliceInsuranceData
        {
            public string Insurance { get; set; }
            public int Level { get; set; }

            public AliceInsuranceData()
            {
                Insurance = "Panam";
                Level = 1;
            }
        }

        public string GetAliceData(string path)
        {
            var url = "https://alice.digital:6984" + "/models/_all_docs";

            var webClient = new WebClient();
            webClient.QueryString.Add("include_docs", "true");
            webClient.Headers.Add("Authorization", "Basic ZWNvbm9taWNzOno2dHJFKnJVRHJ1OA==");
            webClient.Headers.Add("Accept", "application/json");
            webClient.Headers.Add("Content-type", "application/json");
            string result = webClient.DownloadString(url);

            return result;
        }
    }
}
