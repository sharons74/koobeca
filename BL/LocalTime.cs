using System;

namespace KoobecaFeedController.BL {
    public class LocalTime {
        public static DateTime StartTime { get; } = new DateTime(2018, 10, 22); // it all started on my birthday

        public static double CurrentSecond => (DateTime.Now - StartTime).TotalSeconds;

        public static uint CurrentIntSecond => Convert.ToUInt32(CurrentSecond);

        public static DateTime ParseLocalTime(uint localTime) {
            return StartTime.AddSeconds(localTime * 1.0);
        }

        public static DateTime ParseLocalTime(double localTime) {
            return StartTime.AddSeconds(localTime);
        }

        internal static uint ParseDate(DateTime date) {
            if (date > StartTime) return (uint) (date - StartTime).TotalSeconds;

            return 0;
        }
    }
}