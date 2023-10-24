using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using Newtonsoft.Json;

namespace Legacy.LegacyFormatting
{
    public class LegacyOutputFormatter : NewtonsoftJsonOutputFormatter
    {
        public LegacyOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, MvcOptions mvcOptions, MvcNewtonsoftJsonOptions mvcNewtonsoftJsonOptions) : base(serializerSettings, charPool, mvcOptions, mvcNewtonsoftJsonOptions)
        {
            serializerSettings.Converters.Add(new LegacyJsonConverter());
        }
    }
}
