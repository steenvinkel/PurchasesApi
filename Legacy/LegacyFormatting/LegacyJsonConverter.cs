using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var readableProperties = GetReadablePropertyInfos(value.GetType());
            foreach (PropertyInfo prop in readableProperties)
            {
                object propVal = prop.GetValue(value, null);
                if (propVal != null)
                {
                    var name = prop.Name.ToUnderscoreCase();
                    object valt = GetFormattedValue(prop, propVal);
                    var val = JToken.FromObject(valt, serializer);
                    jo.Add(name, val);
                }
            }
            jo.WriteTo(writer);
        }

        private static object GetFormattedValue(PropertyInfo prop, object propVal)
        {
            if (prop.PropertyType.Equals(typeof(int))
                || prop.PropertyType.Equals(typeof(decimal))
                || prop.PropertyType.Equals(typeof(double))
                || prop.PropertyType.Equals(typeof(double?)))
            {
                return propVal.ToString();
            }

            if (prop.PropertyType.Equals(typeof(DateTime)))
            {
                return ((DateTime)propVal).ToString("yyyy-MM-dd");
            }

            return propVal;
        }

        private List<PropertyInfo> GetReadablePropertyInfos(Type type)
        {
            return type.GetProperties().Where(property => property.CanRead).ToList();
        }
    }
}
