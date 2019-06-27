using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Purchases.Helpers;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Purchases.Middleware
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, PurchasesContext dbContext)
        {
            var authToken = context.Request.Cookies["auth_token"];

            var user = dbContext.User.SingleOrDefault(u => u.AuthToken == authToken);
            if (user == null)
            {
                throw new AuthenticationException("The authentication token is invalid");
            }

            if (user.AuthExpire < DateTime.Now)
            {
                throw new AuthenticationException("Authentication token has expired");
            }

            context.Items.Add(HttpContextItemNames.UserId, user.UserId);

            await _next(context);
        }
    }
}
