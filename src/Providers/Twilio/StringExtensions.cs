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
    }
    }
