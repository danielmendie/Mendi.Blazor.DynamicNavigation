using System.Globalization;
using System.Text.RegularExpressions;

namespace CountryApp.Abstractions.Helpers
{
    public class DateHelper
    {
        public static string GetRelativeTime(DateTime date)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeDifference = now - date;

            int years = now.Year - date.Year;
            int months = (now.Year - date.Year) * 12 + now.Month - date.Month;

            // Correct year calculation: Ensure it's actually 12+ months apart
            if (years > 0 && now.Month < date.Month || now.Month == date.Month && now.Day < date.Day)
                years--;

            if (years > 0)
                return $"{years}y ago";
            if (months > 1)
                return $"{months}mth ago";
            if (timeDifference.TotalDays >= 7)
                return $"{(int)(timeDifference.TotalDays / 7)}wk ago";
            if (timeDifference.TotalDays >= 1)
                return $"{(int)timeDifference.TotalDays}d ago";
            if (timeDifference.TotalHours >= 1)
                return $"{(int)timeDifference.TotalHours}h ago";
            if (timeDifference.TotalMinutes >= 1)
                return $"{(int)timeDifference.TotalMinutes}m ago";

            return $"{(int)timeDifference.TotalSeconds}s ago";

        }

        public static string GetRelativeTime(DateTime startDate, DateTime endDate)
        {
            TimeSpan timeDifference = endDate - startDate;

            int years = endDate.Year - startDate.Year;
            int months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;

            // Correct year calculation: Ensure it's actually 12+ months apart
            if (years > 0 && endDate.Month < startDate.Month || endDate.Month == startDate.Month && endDate.Day < startDate.Day)
                years--;

            if (years > 0)
                return $"{years}y";
            if (months > 1)
                return $"{months}mth";
            if (timeDifference.TotalDays >= 7)
                return $"{(int)(timeDifference.TotalDays / 7)}wk";
            if (timeDifference.TotalDays >= 1)
                return $"{(int)timeDifference.TotalDays}d";
            if (timeDifference.TotalHours >= 1)
                return $"{(int)timeDifference.TotalHours}h";
            if (timeDifference.TotalMinutes >= 1)
                return $"{(int)timeDifference.TotalMinutes}m";

            return $"{(int)timeDifference.TotalSeconds}s";

        }

        public static string GetRelativeDate(DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            if (date.Date == today)
                return "Today";

            if (date.Date == yesterday)
                return "Yesterday";

            // Get the start of the current week (assuming week starts on Monday)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            if (date.Date >= startOfWeek && date.Date < today)
                return date.ToString("dddd"); // Returns the day of the week (e.g., "Monday")

            // Return formatted date for past weeks
            return date.ToString("dd - MM - yyyy"); // e.g., "02 - 01 - 2025"
        }

        public static string ConvertMonthNumberToMonthName(int monthNumber, bool showfull = false)
        {
            if (monthNumber < 1 || monthNumber > 12)
                return "Invalid Month";

            string[] months;
            if (showfull)
                months = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "Dececember" };
            else
                months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return months[monthNumber - 1];
        }

        public static string GetCountdown(DateTime targetDate)
        {
            TimeSpan remaining = targetDate - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
                return "Expired";

            string Pluralize(int value, string singular, string plural) =>
                value == 1 ? $"{value}{singular}" : $"{value}{plural}";

            if (remaining.TotalDays > 7)
            {
                int weeks = (int)(remaining.TotalDays / 7);
                int days = remaining.Days % 7;
                int hours = remaining.Hours;
                return $"{Pluralize(weeks, "wk", "wks")} {Pluralize(days, "d", "ds")} {Pluralize(hours, "h", "hs")}";
            }
            else if (remaining.TotalDays > 1)
            {
                int days = remaining.Days;
                int hours = remaining.Hours;
                return $"{Pluralize(days, "d", "ds")} {Pluralize(hours, "h", "hs")}";
            }
            else if (remaining.TotalHours > 1)
            {
                int hours = remaining.Hours;
                int minutes = remaining.Minutes;
                return $"{Pluralize(hours, "h", "hs")} {Pluralize(minutes, "m", "ms")}";
            }
            else
            {
                int minutes = (int)Math.Ceiling(remaining.TotalMinutes);
                return Pluralize(minutes, "m", "ms");
            }
        }

        public static string FormatToHumanReadable(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Replace underscores or hyphens with spaces
            string result = input.Replace("_", " ").Replace("-", " ");

            // Insert spaces before capital letters (for PascalCase or camelCase)
            result = Regex.Replace(result, @"(?<=[a-z])([A-Z])", " $1");

            // Normalize spaces
            result = Regex.Replace(result, @"\s+", " ").Trim();

            // Capitalize first letter of each word
            result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.ToLower());

            return result;
        }
    }
}
