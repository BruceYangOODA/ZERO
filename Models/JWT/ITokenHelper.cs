using System.Security.Claims;

namespace ZERO.Models.JWT
{
    public interface ITokenHelper
    {
        TnToken CreateTokenString(List<Claim> claims, int tokenExpires);
    }
}
