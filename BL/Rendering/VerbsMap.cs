using System.Collections.Generic;

namespace KoobecaFeedController.BL {
    public class VerbsMap {
        private static Dictionary<string, string> _PastVerbs;

        private static void Init() {
            if (_PastVerbs == null) {
                _PastVerbs = new Dictionary<string, string>();
                _PastVerbs["like"] = "liked";
                _PastVerbs["share"] = "shared";
                _PastVerbs["comment"] = "commented on";
                _PastVerbs["react"] = "reacted to";
                _PastVerbs["post"] = "shared";
                _PastVerbs["status"] = "posted";
                _PastVerbs["follow"] = "followed";
            }
        }

        public static string PastVerb(string actionType) {
            Init();

            if (actionType.StartsWith("like")) return _PastVerbs["like"];
            if (actionType.StartsWith("comment")) return _PastVerbs["comment"];
            if (actionType.StartsWith("react")) return _PastVerbs["react"];
            if (actionType.StartsWith("share")) return _PastVerbs["share"];
            if (actionType.Contains("post")) return _PastVerbs["post"];
            if (actionType.Contains("follow")) return _PastVerbs["follow"];

            if (_PastVerbs.TryGetValue(actionType, out var verb))
                return verb;
            return "reacted to";
        }
    }
}