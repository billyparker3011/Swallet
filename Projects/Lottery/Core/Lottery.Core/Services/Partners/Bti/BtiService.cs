using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiService : LotteryBaseService<BtiService>, IBtiSerivice
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private const string SecretKey = "2RMhjk5L/BpViOfKUwj5es68CIQW5JPzpZNzo78mtmSzL4dMlOC58l1AibgkYjhv";
        private readonly SymmetricSecurityKey _signingKey;

        public BtiService(
            ILogger<BtiService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }

        public string GenerateToken(long playerId, DateTime expiryTime)
        {
            var model = new BtiTokenModel()
            {
                PlayerId = playerId,
                ExpiryTime = expiryTime,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("pid", model.PlayerId.ToString()),
                new Claim("exp", model.ExpiryTime.ToString("yyyyMMddHHmmss"))
            }),
                Expires = model.ExpiryTime,
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public BtiOutTokenModel ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                var playerIdClaim = principal.FindFirst("pid")?.Value;
                var expiryTimeClaim = principal.FindFirst("exp")?.Value;

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken != null)
                {
                    var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
                    if (expClaim != null)
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value)).UtcDateTime;
                    }
                }

                if (long.TryParse(playerIdClaim, out var playerId) &&
                    long.TryParse(expiryTimeClaim, out var expiryTime))
                {
                    return new BtiOutTokenModel
                    {
                        PlayerId = playerId,
                        ExpiryTime = DateTimeOffset.FromUnixTimeSeconds(expiryTime).UtcDateTime
                    };
                }
            }
            catch
            {
               
            }
            return new BtiOutTokenModel();
        }

        public async Task<string> GenerateTokenByUsername(string username, DateTime expiryTime)
        {
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var player = await playerRepos.FindQueryBy(c => c.Username == username).FirstOrDefaultAsync();

            if (player == null) return string.Empty;

            var model = new BtiTokenModel()
            {
                PlayerId = player.PlayerId,
                ExpiryTime = expiryTime,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("pid", model.PlayerId.ToString()),
                new Claim("exp", model.ExpiryTime.ToString("yyyyMMddHHmmss"))
            }),
                Expires = model.ExpiryTime,
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task DeleteUserMapping(string username)
        {
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var playerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();
            
            var player = await playerRepos.FindQueryBy(c => c.Username == username).FirstOrDefaultAsync() ?? throw new NotFoundException();

            var playerMapping = await playerMappingRepos.FindQueryBy(c => c.PlayerId == player.PlayerId).FirstOrDefaultAsync() ?? throw new NotFoundException();

            playerMappingRepos.Delete(playerMapping);

           await LotteryUow.SaveChangesAsync();
        }
    }

}
