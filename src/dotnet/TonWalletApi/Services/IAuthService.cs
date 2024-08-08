using TonWalletApi.Dtos;

namespace TonWalletApi.Services
{
    public interface IAuthService
    {
        Task LoginAsync(UserDto user);
        Task<bool> IsUserExist(int id);
        Task<UserDto> GetUserAsync(int id);
    }
}
