using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KoobecaSync.APIs.FB
{
    

    public class FacebookClient
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public FacebookClient(string accessToken)
        {
            _accessToken = accessToken;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v3.2/")
            };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?access_token={_accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
                return default(T);
            
            var result = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<Stream> GetStreamContentAsync(string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?access_token={_accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
                return null;

            return  await response.Content.ReadAsStreamAsync();
        }
    }
}