using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Purchases.Helpers;
using System;
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

        public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
        {
            if (IsAuthenticationController(context))
            {
                await _next(context);
                return;
            }

            var authToken = context.GetAuthToken() ?? throw new AuthenticationException($"The authentication token is invalid");

            if (!_cache.TryGetValue(authToken, out int userId))
            {
                DateTime expiration;
                (userId, expiration) = authenticationService.GetUserIdAndExpiration(authToken);

                _cache.Set(authToken, userId, expiration);
            }

            context.SetUserId(userId);

            await _next(context);
        }

        private static bool IsAuthenticationController(HttpContext context)
        {
            return context.Request.Path.ToString().Contains("/api/authentication") || context.Request.Path.ToString().Contains("/healthz");
        }
    }
}
