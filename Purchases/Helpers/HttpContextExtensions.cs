using Microsoft.AspNetCore.Http;

namespace Purchases.Helpers
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items[HttpContextItemNames.UserId];
        }
    }
}
