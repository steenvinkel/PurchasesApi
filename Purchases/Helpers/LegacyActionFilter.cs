using Legacy.LegacyFormatting;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Purchases.Helpers
{
    public class LegacyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            var formatting = context.HttpContext.Request.Query["formatting"];

            if (formatting == "legacy")
            {
                result.Result.AddLegacyFormatting();
            }
        }
    }
}
