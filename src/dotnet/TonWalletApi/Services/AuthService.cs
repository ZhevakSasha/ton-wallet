using TonWallet.Domain.Entities;
using TonWallet.Domain.Repositories;
using TonWalletApi.Dtos;

namespace TonWalletApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }
        public async Task<bool> IsUserExist(int id)
        {
            return await _userRepository.IsUserExist(id);
        }

        public async Task LoginAsync(UserDto userDto)
        {
            var user = new User { 
                Id = userDto.Id,
                Userame = userDto.Username,
                RAWWalletAddress = userDto.RAWWalletAddress 
            };

             await _userRepository.CreateUser(user);
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            var entity = await _userRepository.GetUserById(id);
            var user = new UserDto { Id = entity.Id, RAWWalletAddress = entity.RAWWalletAddress, Username = entity.Userame };
            return user;
        }
    }
}
