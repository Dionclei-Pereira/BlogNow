using System.Globalization;

namespace System {
    public static class DateTimeExtension {
        public static string ElapsedTime(this DateTime dateTime) {
            dateTime = dateTime.ToUniversalTime();
            TimeSpan duration = DateTime.UtcNow - dateTime;
            if (duration.TotalSeconds < 60) {
                return "Less than a minute ago";
            }
            if (duration.TotalMinutes < 60) {
                return duration.TotalMinutes.ToString("F0", CultureInfo.InvariantCulture) + " minutes ago";
            }
            if (duration.TotalHours < 24.0) {
                return duration.TotalHours.ToString("F1", CultureInfo.InvariantCulture) + " hours ago";
            }
            return duration.TotalDays.ToString("F0", CultureInfo.InvariantCulture) + " days ago";
        }
    }
}
