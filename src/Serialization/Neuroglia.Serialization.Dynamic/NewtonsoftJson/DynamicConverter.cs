using Neuroglia.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Newtonsoft.Json
{

    /// <summary>
    /// Represents the <see cref="JsonConverter"/> used to convert from and to <see cref="Any"/> instances
    /// </summary>
    public class DynamicConverter
        : JsonConverter<Dynamic>
    {

        /// <inheritdoc/>
        public override Dynamic? ReadJson(JsonReader reader, Type objectType, Dynamic? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JObject.ReadFrom(reader);
            if(token == null)
                return null;
            switch (token)
            {
                case JArray jarray:
                    return DynamicArray.FromObject(jarray.ToObject<IList>()!);
                case JObject jobject:
                    return DynamicObject.FromObject(jobject.ToObject<System.Dynamic.ExpandoObject>());
                default:
                    return DynamicValue.FromObject(token.ToObject<object>());
            }
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, Dynamic? value, JsonSerializer serializer)
        {
            if(value != null)
                serializer.Serialize(writer, value.ToObject());
        }

    }

}
