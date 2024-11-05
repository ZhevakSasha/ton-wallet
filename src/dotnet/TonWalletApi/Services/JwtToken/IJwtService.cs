using System.Security.Claims;

namespace TonWalletApi.Services.JwtToken
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, IEnumerable<Claim> additionalClaims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
