using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ZERO.Models.JWT
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IOptions<JWTConfig> _options;
        public TokenHelper(IOptions<JWTConfig> options)
        {
            _options = options;
        }
        /// <summary> 生成token </summary>
        /// <param name="claims"></param>
        /// <param name="tokenExpires"></param>
        /// <returns></returns>
        public TnToken CreateTokenString(List<Claim> claims, int tokenExpires)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Utc);
            var expires = now.Add(TimeSpan.FromMinutes(tokenExpires));
            var userClaimsIdentity = new ClaimsIdentity(claims);
            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                NotBefore = now, // JWT 生效時間的開始時間
                IssuedAt = now, //  JWT 的發行時間
                Subject = userClaimsIdentity,
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.IssuerSigningKey)), SecurityAlgorithms.HmacSha256)
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return new TnToken { tokenStr = new JwtSecurityTokenHandler().WriteToken(securityToken), expires = expires };
        }
    }
}
