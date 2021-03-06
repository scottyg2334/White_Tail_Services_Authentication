using System;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace AuthorizationServer.Services
{

    public class TokenManager : ITokenManager
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IConfiguration _config;

        public TokenManager(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions, IConfiguration config)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
            _config = config;
        }
        public async Task<bool> IsCurrentActiveToken()
        {
           return await IsActiveAsync(GetCurrentAsync());
        }
        public async Task DeactivateCurrentAsync()
        {
            await DeactivateAsync(GetCurrentAsync());
        }
        public async Task<bool> IsActiveAsync(string token)
        {
            return await _cache.GetStringAsync(GetKey(token)) == null;
        }
        public async Task DeactivateAsync(string token)
        {
            await _cache.SetStringAsync(GetKey(token), " ", new DistributedCacheEntryOptions 
            { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }





    private string GetCurrentAsync()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

        if (authorizationHeader == StringValues.Empty)
        {
            return string.Empty;
        }
        else
        {
            return authorizationHeader.Single().Split(" ").Last();
        }
    }

    private static string GetKey(string token)
    {
        return $"tokens:{token}:deactivated";
    }

    }
}