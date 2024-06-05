using System.IdentityModel.Tokens.Jwt;

namespace WebClient.Models.Token
{
    public class JwtUtils
    {
        public static TokenInfo Decode(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenAfter = handler.ReadJwtToken(token);
            var username = tokenAfter.Claims.ToList()[0].Value;
            var userId = tokenAfter.Claims.ToList()[1].Value;
            var role = tokenAfter.Claims.ToList()[2].Value;
            var tokenInfo = new TokenInfo
            {
                Name = username,
                Role = role,
                UserId = userId
            };
            return tokenInfo;
        }
    }
}
