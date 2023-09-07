using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace LineTag.Core.Utility
{
    public static class JsonHelper
    {
        public static T DeserializeObject<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string SerializeObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(value, Formatting.None);
        }

        public static JToken Parse(string json)
        {
            var jToken = JToken.Parse(json);

            SetEmptyToNull(jToken);

            return jToken;
        }

        public static void SetEmptyToNull(JToken jToken)
        {
            if (jToken.Type == JTokenType.Null)
            {
                return;
            }

            if (jToken.Type == JTokenType.Object)
            {
                foreach (var kvp in jToken as JObject)
                {
                    if (kvp.Value.Type == JTokenType.Null)
                    {
                        break;
                    }
                    else if (kvp.Value.ToString() == "{}")
                    {
                        jToken[kvp.Key] = null;
                    }
                    else if (jToken[kvp.Key].Type == JTokenType.Object)
                    {
                        SetEmptyToNull(jToken[kvp.Key]);
                    }
                    else if (jToken[kvp.Key].Type == JTokenType.Array)
                    {
                        SetEmptyToNull(jToken[kvp.Key]);
                    }
                }
            }

            if (jToken.Type == JTokenType.Array)
            {
                if (jToken.ToString() == "[]")
                {
                    return;
                }

                foreach (var item in jToken as JArray)
                {
                    SetEmptyToNull(item);
                }
            }
        }
    }
}