using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HnMicro.Framework.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtToken BuildToken(List<Claim> claims)
        {
            var authenticationValidateOption = _configuration.GetSection(AuthenticationValidateOption.AppSettingName).Get<AuthenticationValidateOption>();
            if (authenticationValidateOption == null)
                throw new UnhandledException("Cannot read AuthenticationValidate option.");

            if (claims.Count == 0)
                throw new UnhandledException("Cannot find any Claims.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationValidateOption.IssuerSigningKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(authenticationValidateOption.ValidIssuer,
                                            authenticationValidateOption.ValidAudience,
                                            claims,
                                            expires: DateTime.UtcNow.AddMinutes(authenticationValidateOption.ExpiryInMinutes),
                                            signingCredentials: credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new JwtToken
            {
                AccessToken = accessToken,
                ExpiresIn = authenticationValidateOption.ExpiryInMinutes,
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };
        }
    }
}
