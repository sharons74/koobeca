using System.Collections.Generic;

namespace KoobecaFeedController.BL {
    public class SourcesMap {
        private readonly Dictionary<string, Dictionary<ulong, object>> _locks =
            new Dictionary<string, Dictionary<ulong, object>>();

        private readonly object _mainLock = new object();

        private readonly Dictionary<string, Dictionary<ulong, SourceData>> _sourcesMap =
            new Dictionary<string, Dictionary<ulong, SourceData>>();

        private SourcesMap() {
            _locks["user"] = new Dictionary<ulong, object>();
            _locks["sitepage_page"] = new Dictionary<ulong, object>();
            _locks["sitegroup_group"] = new Dictionary<ulong, object>();

            _sourcesMap["user"] = new Dictionary<ulong, SourceData>();
            _sourcesMap["sitepage_page"] = new Dictionary<ulong, SourceData>();
            _sourcesMap["sitegroup_group"] = new Dictionary<ulong, SourceData>();
        }

        // Singleton
        public static SourcesMap Instance { get; } = new SourcesMap();


        


        public SourceData GetSource(ulong sourceId, string sourceType) {
            //Logger.Instance.Debug($"Getting source {sourceType}:{sourceId} ");
            lock (_mainLock) {
                if (!_locks.ContainsKey(sourceType)) {
                    Logger.Instance.Warn($"unknown sourcetype {sourceType}");
                    return null; //in case of unsupported sources like "activity_action"
                }

                if (!_locks[sourceType].ContainsKey(sourceId)) _locks[sourceType][sourceId] = new object();
            }

            lock (_locks[sourceType][sourceId]) {
                _sourcesMap[sourceType].TryGetValue(sourceId, out var source);
                if (source == null) {
                    source = DbAccess.Instance.GetSourceData(sourceId, sourceType);
                    _sourcesMap[sourceType][sourceId] = source;
                }
                var ret = _sourcesMap[sourceType][sourceId];
                if(ret == null)
                {
                    Logger.Instance.Warn($"source {sourceType}:{sourceId} not found");
                }
                return ret;
            }
        }

        public Dictionary<string, Dictionary<ulong, List<CategoryInfo>>> GetUpdatedCategories() {
            var result = new Dictionary<string, Dictionary<ulong, List<CategoryInfo>>>();
            result["user"] = new Dictionary<ulong, List<CategoryInfo>>();
            result["sitepage_page"] = new Dictionary<ulong, List<CategoryInfo>>();
            result["sitegoup_group"] = new Dictionary<ulong, List<CategoryInfo>>();


            foreach (var sources in _sourcesMap)
            foreach (var sourcePair in sources.Value) {
                var list = sourcePair.Value.GetUpdatedCategories();
                if (list != null && list.Count > 0) result[sources.Key][sourcePair.Key] = list;
            }

            return result;
        }
    }
}