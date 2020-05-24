using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class TraslateController : ControllerBase
{
    private const string key_var = "TRANSLATOR_TEXT_SUBSCRIPTION_KEY";
    private const string route = "/translate?api-version=3.0&to=en";
    private static readonly string subscriptionKey = Environment.GetEnvironmentVariable(key_var);
    private const string endpoint_var = "TRANSLATOR_TEXT_ENDPOINT";
    private static readonly string endpoint = Environment.GetEnvironmentVariable(endpoint_var);
    private const string region_var = "TRANSLATOR_TEXT_REGION";
    private static readonly string region = Environment.GetEnvironmentVariable(region_var);

    [HttpGet("Traslate/{text}")]
    public async Task<string> TraslateText(string text)
    {
        object[] body = new object[] { new { Text = text } };
        var requestBody = JsonConvert.SerializeObject(body);
        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                List<string> translations = new List<string>();
                foreach (TranslationResult o in deserializedOutput)
                {
                    foreach (Translation t in o.Translations)
                    {
                        translations.Add(t.Text);
                    }
                }
                return JsonConvert.SerializeObject(translations);
            }
        }
    }
}