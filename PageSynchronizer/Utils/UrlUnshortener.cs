using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FB2Koobeca.Utils
{
    public class UrlUnshortener
    {
        public  static string GetRealUrl(string url)
        {
            if (string.IsNullOrEmpty(url))// || url.Length > 40)
                return url;
            try
            {
                int count = 3;
                string resolvedURL = null;
                Exception e;
                do
                {
                    Logger.Instance.Info($"try:{4 - count}");
                    resolvedURL = GetRespose(url, out e);

                } while (resolvedURL == null && count-- > 0);

                if(resolvedURL == null)
                {
                    Logger.Instance.Error($"Failed to resolve url after {count} times,{e.Message}");
                    return url;
                }
                //else
                return resolvedURL;
                
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to get real url:{e.Message}");
                return url;
            }
            
        }

        private static string GetRespose(string url, out Exception exception)
        {
            exception = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                request.Timeout = 5000;
                request.Method = WebRequestMethods.Http.Head;
                WebResponse response = request.GetResponse();
                return response.ResponseUri.ToString();
            }
            catch (Exception e)
            {
                exception = e;
                return null;
            }
        }

    }
}
