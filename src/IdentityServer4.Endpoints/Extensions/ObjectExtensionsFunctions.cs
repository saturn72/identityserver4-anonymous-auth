using System.Text.Json;

namespace System
{
    public static class ObjectExtensionsFunctions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        public static string ToJsonString(this object obj) => JsonSerializer.Serialize(obj, JsonSerializerOptions);

    }
}
