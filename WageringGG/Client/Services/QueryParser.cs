using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;

namespace WageringGG.Client.Services
{
    public class QueryParser
    {
        public Dictionary<string, Microsoft.Extensions.Primitives.StringValues> Values { get; set; }
        public QueryParser(NavigationManager navManager)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            Values = QueryHelpers.ParseQuery(uri.Query);
        }

        public bool TryGetQueryString<T>(string key, out T value)
        {
            if (Values.TryGetValue(key, out var valueFromQueryString))
            {
                if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
                {
                    value = (T)(object)valueAsInt;
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)valueFromQueryString.ToString();
                    return true;
                }

                if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
                {
                    value = (T)(object)valueAsDecimal;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
