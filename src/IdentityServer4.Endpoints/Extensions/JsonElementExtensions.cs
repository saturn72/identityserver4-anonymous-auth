using System.Text.Json;

namespace System
{
    internal static class JsonElementExtensions
    {
        public static bool PropertyStringValueEqualsTo(this JsonElement element, string propertyName, string value)
        => element.GetProperty(propertyName)
                        .GetString()
                        .Equals(value, StringComparison.InvariantCultureIgnoreCase);
    }
}
