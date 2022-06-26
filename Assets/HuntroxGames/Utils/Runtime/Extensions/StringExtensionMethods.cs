using System;
using System.Globalization;

namespace HuntroxGames.Utils
{
    public static class StringExtensionMethods
    {
        
        public static bool IsValid(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        }
        public static string ToTitleCase(this string str) 
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        
        public static string Bold(this string str) => "<b>" + str + "</b>";
        public static string Color(this string str, string color) => $"<color={color}>{str}</color>";
        public static string Italic(this string str) => "<i>" + str + "</i>";
        public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
        public static string Header(this string str) => str.Bold().Color("White").ToUpper();
        public static string SubHeader(this string str) => str.Color("White").ToUpper();
        
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        
        
        public static int ToInt(this string str, int defaultValue = default(int))
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return !int.TryParse(str, out int s) ? s : defaultValue;
        }
        
        public static float ToFloat(this string str, float defaultValue = default(float))
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return !float.TryParse(str, out float s) ? s : defaultValue;
        }
        
        public static bool CaseInsensitiveContains(this string text, string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

    }
}