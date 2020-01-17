using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KoobecaSync.APIs.Koobeca.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KoobecaSync.APIs.Koobeca
{
    
    public class KoobecaClient 
    {
        private readonly HttpClient _httpClient;
        private string _Token = "";
        private string _Secret = "";

        public string Token { 
            set {
                _httpClient.DefaultRequestHeaders.Add("oauth_token", value);
                _Token = value;
            }
        }
        public string Secret
        {
            set
            {
                _httpClient.DefaultRequestHeaders.Add("oauth_secret", value);
                _Secret = value;
            }
        }

        public KoobecaClient()
        {
            _httpClient = new HttpClient
            {
                //BaseAddress = new Uri("https://localhost:36812/api/rest/")
                BaseAddress = new Uri("https://beta.koobeca.com/api/rest/")
            };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?oauth_token={_Token}&oauth_secret={_Secret}&{args}");
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();
           

            return JsonConvert.DeserializeObject<T>(result);
        }

        //public async Task<string> GetAsyncStr(string endpoint, string args = null)
        //{
        //    var response = await _httpClient.GetAsync($"{endpoint}?oauth_token={_Token}&oauth_secret={_Secret}&{args}");
        //    if (!response.IsSuccessStatusCode)
        //        return null;

        //    var result = await response.Content.ReadAsStringAsync();
        //    return result;
        //}

        public async Task<T> PostAsync<T>(string endpoint, object data, string args = null)
        {
            var payload = GetPayload(data);
            var response = await _httpClient.PostAsync($"{endpoint}?oauth_token={_Token}&oauth_secret={_Secret}&{args}", payload);

            if (!response.IsSuccessStatusCode)
                return default(T);
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }

        //public async Task<T> PostAsync<T>(string endpoint, string args = null)
        //{
        //    var response = await _httpClient.PostAsync($"{endpoint}?oauth_token={_Token}&oauth_secret={_Secret}&{args}", null);
        //    //var response = await _httpClient.PostAsync($"", payload);

        //    if (!response.IsSuccessStatusCode)
        //        return default(T);
        //    var result = await response.Content.ReadAsStringAsync();

        //    return JsonConvert.DeserializeObject<T>(result);
        //}
        public async Task<T> PostReqAsync<T>(string endpoint, KoobecaApiRequest req, string args = null)
        {
            var formData = new MultipartFormDataContent {
                {new StringContent(req.body??""), "body"},
                {new StringContent(req.auth_view), "auth_view"},
            };

            var props = req.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<KoobecaParamAttribute>();
                if (attr == null) continue;
                var val = prop.GetValue(req)?.ToString();
                var key = prop.Name;
                if (val != null)
                {
                    formData.Add(new StringContent(val), key);
                }
            }

            if (req.photos != null)
            {
                formData.Add(new StringContent("photo"), "type");
                if (req.photos.Length == 1)
                {
                    formData.Add(new StreamContent(req.photos[0]), $"photo", $"photo.png");
                }
                else
                {
                    int i = 0;
                    foreach (var photoStrem in req.photos)
                    {
                        formData.Add(new StreamContent(photoStrem), $"photo{i++}", $"photo.png");
                    }
                }
            }

            var response = await _httpClient.PostAsync($"{endpoint}?oauth_token={_Token}&oauth_secret={_Secret}&{args}", formData);

            if (!response.IsSuccessStatusCode)
                return default(T);
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }


        private IDictionary<string, string> ToKeyValue(object metaToken) {
            if (metaToken == null) {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null) {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues) {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList()) {
                    var childContent = ToKeyValue(child);
                    if (childContent != null) {
                        contentData = contentData.Concat(childContent)
                            .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null) {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                jValue?.ToString("o", CultureInfo.InvariantCulture) :
                jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }

        private FormUrlEncodedContent GetPayload(object data) {
            if (data == null) return null;
            var frm = new FormUrlEncodedContent(ToKeyValue(data));
            return frm;
        }

    }
}
