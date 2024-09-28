using TonWalletApi.Dtos;

namespace TonWalletApi.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(UserDto user);
        Task<bool> IsUserExist(int id);
        Task<UserDto> GetUserAsync(int id);
        Task LogoutAsync(int id);
        Task<AuthResult> RefreshTokenAsync(TokenDto tokenDto);
    }
}
