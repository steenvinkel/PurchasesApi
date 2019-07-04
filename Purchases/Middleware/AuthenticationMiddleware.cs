using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public CustomAuthenticationMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, PurchasesContext dbContext)
        {
            if (IsAuthenticationController(context))
            {
                await _next(context);
                return;
            }

            string authToken = context.GetAuthToken();

            if (!_cache.TryGetValue(authToken, out int userId))
            {
                DateTime expiration;
                (userId, expiration) = GetUserIdAndExpiration(dbContext, authToken);

                _cache.Set(authToken, userId, expiration);
            }

            context.SetUserId(userId);

            await _next(context);
        }

        private static bool IsAuthenticationController(HttpContext context)
        {
            return context.Request.Path == "/api/authentication";
        }

        private Tuple<int, DateTime> GetUserIdAndExpiration(PurchasesContext dbContext, string authToken)
        {
            var user = dbContext.User.SingleOrDefault(u => u.AuthToken == authToken);
            if (user == null)
            {
                throw new AuthenticationException($"The authentication token ({authToken}) is invalid");
            }

            if (user.AuthExpire < DateTime.Now)
            {
                throw new AuthenticationException("Authentication token has expired");
            }

            return Tuple.Create(user.UserId, user.AuthExpire);
        }
    }
}
