using Legacy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Legacy.LegacyFormatting
{
    public class LegacyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return !objectType.IsGenericType && objectType.IsSubclassOf(typeof(object)) && !typeof(string).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            Type type = value.GetType();

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead)
                {
                    object propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        var name = prop.Name.ToUnderscoreCase();
                        var valt = prop.PropertyType.Equals(typeof(int)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(Double))
                            ? propVal.ToString()
                            : propVal;
                        var val = JToken.FromObject(valt, serializer);
                        jo.Add(name, val);
                    }
                }
            }
            jo.WriteTo(writer);
        }
    }
}
