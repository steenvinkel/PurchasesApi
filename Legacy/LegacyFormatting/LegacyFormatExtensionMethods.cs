using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Buffers;

namespace Legacy.LegacyFormatting
{
    public static class LegacyFormatExtensionMethods
    {
        public static ObjectResult AddLegacyFormatting(this object result)
        {
            var objectResult = new ObjectResult(result);

            objectResult.AddLegacyFormatting();

            return objectResult;
        }

        public static IActionResult AddLegacyFormatting(this IActionResult result)
        {
            if (result is ObjectResult objectResult)
            {
                objectResult.Formatters.Add(CreateLegacyFormatter());
            }

            return result;
        }

        public static ObjectResult AddLegacyFormatting(this ObjectResult objectResult)
        {
            objectResult.Formatters.Add(CreateLegacyFormatter());

            return objectResult;
        }

        private static IOutputFormatter CreateLegacyFormatter()
        {
            var serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            return new LegacyOutputFormatter(serializerSettings, ArrayPool<char>.Shared, new MvcOptions());
        }
    }
}
