using System;
using Microsoft.AspNetCore.Http;

namespace Purchases.Helpers
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext context)
        {
            var userId = (int?)context.Items[HttpContextItemNames.UserId];

            return userId == null
                ? throw new Exception("Could not find userId")
                : userId.Value;
        }

        public static void SetUserId(this HttpContext context, int userId)
        {
            context.Items.Add(HttpContextItemNames.UserId, userId);
        }

        public static string? GetAuthToken(this HttpContext context)
        {
            var authToken = context.Request.Cookies["auth_token"] ?? context.Request.Headers["auth_token"];

            return authToken;
        }
    }
}
