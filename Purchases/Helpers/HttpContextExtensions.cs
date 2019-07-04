using Microsoft.AspNetCore.Http;

namespace Purchases.Helpers
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items[HttpContextItemNames.UserId];
        }

        public static void SetUserId(this HttpContext context, int userId)
        {
            context.Items.Add(HttpContextItemNames.UserId, userId);
        }

        public static string GetAuthToken(this HttpContext context)
        {
            var authToken = context.Request.Cookies["auth_token"];
            if (authToken == null)
            {
                authToken = context.Request.Headers["auth_token"];
            }

            return authToken;
        }
    }
}
