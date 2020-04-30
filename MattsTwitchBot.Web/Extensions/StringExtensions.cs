using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MattsTwitchBot.Web.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSaneJsonForType<T>(this string @json)
        {
            // if it's empty or null, it's not "sane" to begin with (though empty string may be VALID json, it's not SANE)
            if (string.IsNullOrEmpty(json))
                return false;

            // run the string into a C# object and then back out
            var toObjAndBackToJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<T>(json));

            // if there are any differences (ignoring whitespace), it's not sane
            // adding a field that shouldn't be there, for instance, like a typo
            var obj1 = JObject.Parse(json);
            var obj2 = JObject.Parse(toObjAndBackToJson);
            return JToken.DeepEquals(obj1, obj2);
        }
    }
}