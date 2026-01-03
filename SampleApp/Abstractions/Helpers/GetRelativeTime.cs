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

        public static string ConvertMonthNumberToMonthName(int monthNumber, bool showfull = false)
        {
            if (monthNumber < 1 || monthNumber > 12)
                return "Invalid Month";

            string[] months;
            if (showfull)
                months = ["January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "Dececember"];
            else
                months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            return months[monthNumber - 1];
        }

        public static string FormatToHumanReadable(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string result = input.Replace("_", " ").Replace("-", " ");
            result = Regex.Replace(result, @"(?<=[a-z])([A-Z])", " $1");
            result = Regex.Replace(result, @"\s+", " ").Trim();
            result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.ToLower());
            return result;
        }
    }
}
