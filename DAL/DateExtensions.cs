using System;

namespace KoobecaFeedController.DAL {
    internal static class DateExtensions {
        public static DateTime Spec(this DateTime d) {
            return d.Kind != DateTimeKind.Unspecified ? d : DateTime.SpecifyKind(d, DateTimeKind.Utc);
        }
    }
}