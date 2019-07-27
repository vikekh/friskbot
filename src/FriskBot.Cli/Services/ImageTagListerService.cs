using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;


namespace FriskBot.Cli.Services
{
    class ImageTagListerService
    {
        static DateTime _lastRequest;
        static object _warlock = new object();

        private partial class VisionResult
        {
            [JsonProperty("description")]
            public Description Description { get; set; }

            [JsonProperty("requestId")]
            public Guid RequestId { get; set; }

            [JsonProperty("metadata")]
            public Metadata Metadata { get; set; }
        }

        private partial class Description
        {
            [JsonProperty("tags")]
            public string[] Tags { get; set; }

            [JsonProperty("captions")]
            public Caption[] Captions { get; set; }
        }

        private partial class Caption
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("confidence")]
            public double Confidence { get; set; }
        }

        private partial class Metadata
        {
            [JsonProperty("width")]
            public long Width { get; set; }

            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("format")]
            public string Format { get; set; }
        }

        public static async void flerg()
        {
            lock(_warlock) {
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5d7c8b589b264b80aa0c2a6ff1752e1e");

                // Request parameters
                queryString["maxCandidates"] = "1";
                queryString["language"] = "en";
                var uri = "https://friskbot-vision.cognitiveservices.azure.com/vision/v2.0/describe?" + queryString;

                HttpResponseMessage response;

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { url = "https://cdn.discordapp.com/attachments/503278200064049152/604058857194389504/FByFMr8.jpg" }));

                using (var content = new ByteArrayContent(byteData)) {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = client.PostAsync(uri, content).Result;

                    _lastRequest = DateTime.Now;
                }

                try {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var visionResult = Newtonsoft.Json.JsonConvert.DeserializeObject<VisionResult>(content);
                } catch (Exception exc) {

                }
            }
        }
    }
}
