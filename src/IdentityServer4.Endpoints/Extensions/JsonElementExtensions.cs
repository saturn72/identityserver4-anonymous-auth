using System.Text.Json;

namespace System
{
    public static class JsonElementExtensions
    {
        public static bool PropertyStringValueEqualsTo(
            this JsonElement element,
            string propertyName,
            string value,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (!propertyName.HasValue())
                throw new ArgumentException(nameof(propertyName));

            var p = element.GetProperty(propertyName);
            return p.GetString().Equals(value, comparison);
        }
    }
}
