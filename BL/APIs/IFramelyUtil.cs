using KoobecaFeedController.DAL.Adapters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace KoobecaFeedController.BL.APIs
{
    public class IFramelyUtil
    {
        private static string _apiUrlTemplate = "https://iframe.ly/api/iframely?url={0}&api_key={1}";

        public static string Get(string url)
        {
            try
            {
                string apiUrl = string.Format(_apiUrlTemplate, System.Web.HttpUtility.HtmlEncode(url), CoreSettings.Get("core.iframely.secretIframelyKey"));
                var httpClient = new HttpClient
                {
                    // BaseAddress = new Uri(apiUrl)
                };
                httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return httpClient.GetAsync(apiUrl).Result.Content.ReadAsStringAsync().Result;
            }
            catch(Exception e)
            {
                return null;
            }
       }
    }
}
