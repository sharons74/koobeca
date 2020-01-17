namespace KoobecaFeedController.BL {
    public class GlobalSettings {
        public static readonly double IncreaseFactorForActiveUsers = 1.05; //5% increase
        public static readonly double IncreaseFactorForInactiveUsers = 1.5; //50% increase
        public static readonly byte AffinityMinimalStep = 1;
        public static readonly byte MaxAffinityValue = 100;
        public static readonly byte AffinityDecreaseStep = 1;
        public static readonly double IncreaseFactorForMemberAffinity = 1.05;
        public static readonly byte MemberAffinityStep = 1;
        public static readonly uint MemberAffinityInactivityTimeInSeconds = 3600 * 24; //one day;

        /// <summary>
        ///     20
        /// </summary>
        public static readonly byte
            MinimalAffinityForAutoDecrease = 20; //we do not go down from 20 in automatic decrease

        public static readonly uint CategoryAffinityInactivityTimeInSeconds = 3600 * 24; //one day

        public static readonly uint
            ActiveUserMaximalInactivityTime = 3600 * 48; //beyond that user is considered inactive user 

        public static readonly byte InitialMemberAffinity = 50;
    }
}