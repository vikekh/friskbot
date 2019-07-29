using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace FriskBot.Cli.Services
{
    class SentimentService
    {
        public static async void flerg()
        {
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
                new { documents = new[] { new { language = "en", id = "1", text = "no one cares" } } }));

            using (var content = new ByteArrayContent(byteData)) {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            try {
                var content = await response.Content.ReadAsStringAsync();
            } catch (Exception exc) {

            }
        }

        /*			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "360ec6c860804dc1992989f1cec7375e");

			// Request parameters
			queryString["showStats"] = "false";
			var uri = "https://ai-text.cognitiveservices.azure.com/text/analytics/v2.1/sentiment?" + queryString;

			HttpResponseMessage response;

			// Request body
			byte[] byteData = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(
				new { documents = new[] { new { language = "sv", id = "1", text = "jättebra! nu mördar vi stål" } } }));*/
    }
}
