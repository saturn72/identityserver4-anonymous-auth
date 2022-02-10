using System.Text.Json;

namespace System
{
    internal static class StringExtensions
    {
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
        public static bool IsLocalUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // Allows "/" or "/foo" but not "//" or "/\".
            if (url[0] == '/')
            {
                // url is exactly "/"
                if (url.Length == 1)
                {
                    return true;
                }

                // url doesn't start with "//" or "/\"
                if (url[1] != '/' && url[1] != '\\')
                {
                    return true;
                }

                return false;
            }

            // Allows "~/" or "~/foo" but not "~//" or "~/\".
            if (url[0] == '~' && url.Length > 1 && url[1] == '/')
            {
                // url is exactly "~/"
                if (url.Length == 2)
                {
                    return true;
                }

                // url doesn't start with "~//" or "~/\"
                if (url[2] != '/' && url[2] != '\\')
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
