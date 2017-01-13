using System;
using System.Globalization;
using System.Text;

namespace SellerNotesBot
{
    public class Utils
    {
        /// <summary>
        /// Supported keywords: včera, pondělí - neděle, včetně i bez diakritiky
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? CzechDayToDate(string value)
        {
            var result = DayToDate(DayToEnglish(value));
            return result;
        }
        /// <summary>
        /// Supported keywords: yesterday, monday, tuesday, .., sunday
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? DayToDate(string value)
        {
            var currentDay = DateTime.Today;

            if (value?.ToLower() == "yesterday")
            {
                return currentDay.AddDays(-1);
            }

            for (int i = 0; i < 7; i++)
            {
                if (currentDay.DayOfWeek.ToString().ToLower() == value?.ToLower())
                {
                    return currentDay;
                }
                currentDay = currentDay.AddDays(-1);
            }
            return null;
        }
        public static string DayToEnglish(string value)
        {
            string result;
            var key = RemoveDiacritics(value?.ToLower());
            switch (key)
            {
                case "pondeli":
                    result = "monday";
                    break;
                case "utery":
                    result = "tuesday";
                    break;
                case "streda":
                    result = "wednesday";
                    break;
                case "ctvrtek":
                    result = "thursday";
                    break;
                case "patek":
                    result = "friday";
                    break;
                case "sobota":
                    result = "saturday";
                    break;
                case "nedele":
                    result = "sunday";
                    break;
                case "vcera":
                    result = "yesterday";
                    break;
                default:
                    result = value;
                    break;
            }
            return result;
        }
        public static string RemoveDiacritics(string input)
        {
            if (input == null)
                return null;
            input = input.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(input[i]) != UnicodeCategory.NonSpacingMark) sb.Append(input[i]);
            }

            return sb.ToString();
        }
    }
}