using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KoobecaFeedController.BL {
    public class HashTagFinder {
        public static List<string> FindTags(string input) {
            var result = new List<string>();

            foreach (Match match in Regex.Matches(input, "#[a-zA-Z0-9_]+")) result.Add(match.Value);

            return result.GroupBy(test => test).Select(grp => grp.First()).ToList() ?? new List<string>();

            //return result;
        }
    }
}