using System;
using System.Linq;

namespace IdentityServer4.Models
{
    public static class ClientExtensions
    {
        public static bool TryGetBooleanPropertyOrDefault(this Client client, string propertyName, bool defaultValue = default)
        {
            var res = defaultValue;
            _ = ValidateParametersAndVaraibles(client, propertyName, out string value) &&
                bool.TryParse(value, out res);

            return res;
        }
        public static int TryGetIntPropertyOrDefault(this Client client, string propertyName, int defaultValue = default)
        {
            var res = defaultValue;
            _ = ValidateParametersAndVaraibles(client, propertyName, out string value) &&
                int.TryParse(value, out res);

            return res;
        }
        public static string TryGetStringPropertyOrDefault(this Client client, string propertyName, string defaultValue = default)
        {
            return ValidateParametersAndVaraibles(client, propertyName, out string value) ?
                value :
                defaultValue;
        }
        private static bool ValidateParametersAndVaraibles(Client client, string propertyName, out string value)
        {
            value = "";
            var res =
            client?.Properties?.Any() == true &&
            propertyName.HasValue() &&
            client.Properties.TryGetValue(propertyName, out value) &&
            value.HasValue();
            return res;
        }
    }
}
