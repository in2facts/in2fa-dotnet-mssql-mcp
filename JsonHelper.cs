using Newtonsoft.Json.Linq;

namespace mssqlMCP;

public static class JsonHelper
{
    public static JToken? GetCaseInsensitive(this JObject? obj, params string[] path)
    {
        if (obj == null) return null;

        JToken current = obj;
        foreach (var key in path)
        {
            if (current is JObject jObj)
            {
                var prop = jObj.Properties()
                    .FirstOrDefault(p => string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                    return null;

                current = prop.Value;
            }
            else
            {
                return null;
            }
        }

        return current;
    }
}