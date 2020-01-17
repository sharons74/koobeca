using System.Collections.Generic;

namespace KoobecaFeedController.BL {
    internal class ActivityFilter {
        private readonly Dictionary<byte, Dictionary<byte, byte>> _affinity =
            new Dictionary<byte, Dictionary<byte, byte>>();

        private readonly List<ulong> _filteredItems = new List<ulong>();
        private readonly ulong _languages;
        private readonly ulong[] _subcategories = new ulong[64];

        private ulong _categories;


        public ActivityFilter(IEnumerable<CategoryInfo> categories, ulong languages) {
            SetCategories(categories);
            _languages = languages;
        }

        //public void SetCategories(IEnumerable<CategoryInfo> categories) {
        //    foreach (var category in categories) {
        //        //calculate if we want this category based on affinity
        //        var negativity = RandomBytes.Rand.Next(60);
        //        if (category.AffinityLevel - negativity - 20 > 0) {
        //            // we take this subcategory
        //            _subcategories[category.Category] |= (ulong)1 << category.Subcategory;
        //            _categories |= (ulong)1 << category.Category;
        //        }
        //    }
        //}

        public void SetCategories(IEnumerable<CategoryInfo> categories) {
            foreach (var category in categories) {
                _categories |= (ulong) 1 << category.Category;
                _subcategories[category.Category] |= (ulong) 1 << category.Subcategory;
                if (!_affinity.ContainsKey(category.Category))
                    _affinity[category.Category] = new Dictionary<byte, byte>();
                _affinity[category.Category][category.Subcategory] = category.AffinityLevel;
            }
        }

        public bool FilterByCategories(Activity activity) {
            if ((((ulong) 1 << activity.Category) & _categories) == 0) return true;

            if ((((ulong) 1 << activity.Subcategory) & _subcategories[activity.Category]) == 0) return true;

            //match by category & subcategory.
            //now we cake affinity into account
            var affinity = _affinity[activity.Category][activity.Subcategory];
            var random = (byte) RandomBytes.Rand.Next(GlobalSettings.MaxAffinityValue - 1);
            return affinity <= random; // the value of the affinity (0-100) represents it's probability to be taken
        }

        public bool Filter(Activity activity) {
            //fast check if this item was already filtered
            if (_filteredItems.Contains(activity.ActivityId)) return true;

            if (activity.Privacy == Privacy.OnlyMe) return true;

            var filter = FilterLanguage(activity) || FilterByCategories(activity);

            if (filter) _filteredItems.Add(activity.ActivityId);

            return filter;
        }

        private bool FilterLanguage(Activity activity) {
            return (activity.Languages & _languages) == 0;
        }
    }
}