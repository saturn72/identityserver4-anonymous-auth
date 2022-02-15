using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace System
{
    public static class StringExtensions
    {
        public static bool HasValue(this string source)
            => !string.IsNullOrEmpty(source) && !string.IsNullOrWhiteSpace(source);

        public static bool TryParseAsJsonDocument(this string json, out JsonDocument value)
        {
            value = default;
            try
            {
                value = JsonDocument.Parse(json);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static string EnsureLeadingSlash(this string path) => path[0] == '/' ? path : '/' + path;
        public static string EnsureTrailingSlash(this string url) => url[^1] == '/' ? url : url + '/';
        public static string RemoveTrailingSlash(this string url) => url[^1] == '/' ? url[..^1] : url;
        public static bool TryConvertToUri(this string uri, out Uri value)
        {
            try
            {
                value = new Uri(uri);
                return true;
            }
            catch
            {
                value = default;
            }
            return false;
        }
        public static IEnumerable<string> FromDelimitedString(this string input, string delimiter)
        {
            if (input == default)
                return default;
            if (!delimiter.HasValue())
                return new[] { input };

            input = input.Trim();
            return input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string ToDelimitedString(this IEnumerable<string> source, string delimiter = "")
        {
            if (source == null)
                return null;

            if (delimiter == null) delimiter = string.Empty;

            var len = source.Count();
            if (len == 0)
                return string.Empty;

            var sb = new StringBuilder(100);
            var i = 0;
            while (i < len - 1)
            {
                sb.Append(source.ElementAt(i) + delimiter);
                i++;
            }
            sb.Append(source.ElementAt(i));

            return sb.ToString();
        }
    }
}
