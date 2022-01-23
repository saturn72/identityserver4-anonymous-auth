using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToScrubbedDictionary(this NameValueCollection collection, params string[] nameFilter)
        {
            var dict = new Dictionary<string, string>();

            if (collection == null || collection.Count == 0)
            {
                return dict;
            }

            foreach (string name in collection)
            {
                var value = collection.Get(name);
                if (value != null)
                {
                    if (nameFilter.Contains(name, StringComparer.OrdinalIgnoreCase))
                    {
                        value = "***REDACTED***";
                    }
                    dict.Add(name, value);
                }
            }

            return dict;
        }
    }
}
