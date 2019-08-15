using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FriskBot.Cli.Services
{
    class SentimentService
    {
        static DateTime _lastRequest;
        static object _warlock = new object();

        public partial class SentimentResult
        {
            [JsonProperty("documents")]
            public Document[] Documents { get; set; }

            [JsonProperty("errors")]
            public object[] Errors { get; set; }
        }

        public partial class Document
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("score")]
            public double Score { get; set; }
        }

        public static async Task<double> GetSentiment(string text)
        {
            lock (_warlock) {
                if ((DateTime.Now - _lastRequest).TotalMilliseconds < 100) {
                    System.Threading.Thread.Sleep(50);
                }

                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("AZURE_LUIS_API_KEY"));

                // Request parameters
                queryString["showStats"] = "false";
                var uri = "https://ai-text.cognitiveservices.azure.com/text/analytics/v2.1/sentiment?" + queryString;

                HttpResponseMessage response;

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { documents = new[] { new { language = "sv", id = "1", text } } }));

                using (var content = new ByteArrayContent(byteData)) {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = client.PostAsync(uri, content).Result;
                }

                try {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sentimentResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SentimentResult>(content);

                    return sentimentResult.Documents[0].Score;
                } catch (Exception exc) {

                    return 0;
                }
            }
        }
    }
}
