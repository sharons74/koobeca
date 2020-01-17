using System;

namespace KoobecaFeedController.BL {
    public class NetworkMember {
        private byte _affinityLevel = 50;
        private static readonly string[] TypeMap = new string[3] {"user", "sitepage_page", "sitegroup_group"};
        public ulong MemberId { get; set; }
        public NetworkMemberType MemberType = NetworkMemberType.User;

        public bool MemberIs(string type)
        {
            return type.Contains(MemberType.ToString().ToLower());
        }

        public byte AffinityLevel {
            get => _affinityLevel;
            set {
                _affinityLevel = value > GlobalSettings.MaxAffinityValue ? GlobalSettings.MaxAffinityValue : value;

                LastAffinityUpdate = LocalTime.CurrentIntSecond;
            }
        }

        public uint LastAffinityUpdate { get; private set; }
        public string Type
        {
            get { return TypeMap[(int)MemberType]; }
            set
            {
                switch (value)
                {
                    case "sitepage_page":
                        MemberType = NetworkMemberType.Page;
                        break;
                    case "sitegroup_group":
                        MemberType = NetworkMemberType.Group;
                        break;
                    default:
                        MemberType = NetworkMemberType.User;
                        break;
                }
            }
        }

        public SourceData Source {
            get
            {
                return SourcesMap.Instance.GetSource(MemberId, Type);
            }
        }

        public bool IncreaseAffinity() {
            AffinityLevel = Convert.ToByte(Math.Max(AffinityLevel * GlobalSettings.IncreaseFactorForMemberAffinity,
                AffinityLevel + GlobalSettings.MemberAffinityStep));
            return true;
        }

        public bool DecreaseAffinity() {
            if (AffinityLevel <= GlobalSettings.MinimalAffinityForAutoDecrease)
                return false; // we do not decrease to less than this , only by explicit demand

            // decrease affinity if more than "inactivity period" passed since last affinity update
            if (LocalTime.CurrentIntSecond - LastAffinityUpdate >
                GlobalSettings.MemberAffinityInactivityTimeInSeconds) {
                AffinityLevel = Convert.ToByte(Math.Max(AffinityLevel - GlobalSettings.AffinityDecreaseStep, 0));
                return true;
            }

            return false;
        }
    }

    public enum NetworkMemberType {
        User,
        Page,
        Group
    }
}