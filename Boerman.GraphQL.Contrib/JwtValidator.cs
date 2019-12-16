using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Boerman.GraphQL.Contrib
{
    public class JwtValidator
    {
        private readonly TokenValidationParameters _validationParams;

        public JwtValidator(TokenValidationParameters validationParams)
        {
            _validationParams = validationParams;
        }

        public ClaimsPrincipal ValidateJwt(string jwt)
        {
            if (String.IsNullOrWhiteSpace(jwt)) return null;

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var user = handler.ValidateToken(jwt, _validationParams, out var _);
            return user;
        }
    }
}
