using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using LMS.Infrastructure.Common.Config;
using System.Security.Claims;
using System;


namespace LMS.Infrastructure.Common
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ILogger<IJwtTokenService> logger;

        private readonly GuestUserConfig guestUserConfig;

        public JwtTokenService(ILogger<JwtTokenService> logger, IOptions<GuestUserConfig> guestUserConfig)
        {
            this.logger = logger;
            this.guestUserConfig = guestUserConfig.Value;
        }

        public UserClaimDataModel GetClaimData(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("GuestUserKey"))
            {
                httpContext.Request.Headers.TryGetValue("GuestUserKey", out var value);
                if (value == guestUserConfig.GuestUserKey)
                {
                    return new GuestUserClaimDataModel();
                }
            }

            StringValues stringValues = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(stringValues))
            {
                throw new UnauthorizedAccessException("request header can not be null : GetJWTToken : context.Request.Headers[Authorization][0]");
            }

            string text = stringValues[0].Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(text))
            {
                throw new UnauthorizedAccessException("token can not be null : GetJWTToken : header.Substring(Bearer .Length)");
            }

            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(text);
            UserClaimDataModel userClaimDataModel = new UserClaimDataModel
            {
                JwtToken = text
            };
            foreach (Claim claim in jwtSecurityToken.Claims)
            {
                string text2 = claim.Type.ToLowerInvariant();
                switch (text2)
                {
                    case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":
                        userClaimDataModel.Email = claim.Value;
                        break;
                    case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":
                        userClaimDataModel.Username = claim.Value;
                        break;

                }
            }

            logger.LogInformation("JwtTokenService/GetClaimData ClaimDataModel - {claimDataModel}", userClaimDataModel);
            return userClaimDataModel;
        }

    }
}
