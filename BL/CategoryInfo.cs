using System;

namespace KoobecaFeedController.BL {
    public class CategoryInfo {
        private byte _affinityLevel;
        private uint _lastAffinityUpdate;

        public CategoryInfo() {
            _lastAffinityUpdate = LocalTime.CurrentIntSecond;
        }

        public byte Category { get; set; }
        public byte Subcategory { get; set; }

        public byte AffinityLevel {
            get => _affinityLevel;
            set {
                _affinityLevel = value > GlobalSettings.MaxAffinityValue ? GlobalSettings.MaxAffinityValue : value;
                _lastAffinityUpdate = LocalTime.CurrentIntSecond;
            }
        }

        public void IncreaseAffinity(bool activeUser) {
            //increase by 5%(or 50%) or 1 (the bigger one)
            var increaseFactore = activeUser
                ? GlobalSettings.IncreaseFactorForActiveUsers
                : GlobalSettings
                    .IncreaseFactorForInactiveUsers; // if user wake up after long time and like this category, we give it more weight
            AffinityLevel = Convert.ToByte(Math.Max(AffinityLevel * increaseFactore,
                AffinityLevel + GlobalSettings.AffinityMinimalStep));
        }

        public void DecreaseAffinity(bool activeUser) {
            // decrease affinity if more than a 1 days passed since last affinity update & the user is active
            var inactiveCategory = LocalTime.CurrentIntSecond - _lastAffinityUpdate >
                                   GlobalSettings.CategoryAffinityInactivityTimeInSeconds;
            if (activeUser && inactiveCategory)
                AffinityLevel = (byte) Math.Max(AffinityLevel - GlobalSettings.AffinityDecreaseStep, 0);
            else if (!activeUser)
                _lastAffinityUpdate = LocalTime.CurrentIntSecond; // we the past inactivity and start recounting
        }

        public bool CheckAffinityUpdate(uint referenceTime) {
            return _lastAffinityUpdate > referenceTime;
        }
    }
}