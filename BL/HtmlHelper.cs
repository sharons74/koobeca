using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KoobecaFeedController.BL
{
    public class HtmlHelper
    {
        public static string WrapLinksWithATag(string origStr, out string url)
        {
            url = null;
            var retHtml = origStr;
            var linkParser = new Regex(@"\b(bit.ly/|goo.gl/)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(origStr))
            {
                url = $"http://{m.Value}";
                retHtml = retHtml.Replace(m.Value, $"<a href=\"http://{m.Value}\">{m.Value}</a>");
            }

            return retHtml;

        }

        public static string FindUrl(string inputStr)
        {
            var linkParser = new Regex(@"\b((bit.ly/|goo.gl/|http::/|https://)\S+\/?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = linkParser.Match(inputStr);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
    }
}
