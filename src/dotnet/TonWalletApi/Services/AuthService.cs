using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using TonWallet.Domain.Entities;
using TonWallet.Domain.Repositories;
using TonWalletApi.Dtos;
using TonWalletApi.Services.Caching;
using TonWalletApi.Services.JwtToken;

namespace TonWalletApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService _cacheService;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IHttpClientFactory httpClientFactory, ICacheService cacheService, IJwtService jwtService) 
        {
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
            _jwtService = jwtService;
        }
        public async Task<bool> IsUserExist(int id)
        {
            return await _userRepository.IsUserExist(id);
        }

        public async Task<AuthResult> LoginAsync(UserDto userDto)
        {
            var refreshToken = _jwtService.GenerateRefreshToken();
            var token = _jwtService.GenerateAccessToken(userDto.Id);
            await _cacheService.SetCacheAsync(userDto.Id.ToString(), refreshToken, TimeSpan.FromDays(15));

            if (await _userRepository.IsUserExist(userDto.Id))
            {
                return new AuthResult { AccessToken = token, RefreshToken = refreshToken };
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://tonapi.io/v2/address/{userDto.WalletAddress}/parse";
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var walletAddreses = JsonConvert.DeserializeObject<WalletAddressDto>(responseString);

            var user = new User { 
                Id = userDto.Id,
                Username = userDto.Username,
                WalletAddresses = new TonWallet.Domain.Entities.WalletAddress
                {
                    RawForm = userDto.WalletAddress,
                    Bounceable = new TonWallet.Domain.Entities.AddressInfo { B64 = walletAddreses.Bounceable.B64, B64url = walletAddreses.Bounceable.B64url },
                    NonBounceable = new TonWallet.Domain.Entities.AddressInfo { B64 = walletAddreses.NonBounceable.B64, B64url = walletAddreses.NonBounceable.B64url }
                }
            };

             await _userRepository.CreateUser(user);

            return new AuthResult { AccessToken = token, RefreshToken = refreshToken };
        }

        public async Task LogoutAsync(int id)
        {
            await _userRepository.DeleteUser(id);
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            var entity = await _userRepository.GetUserById(id);
            var user = new UserDto { Id = entity.Id, WalletAddress = entity.WalletAddresses.NonBounceable.B64url, Username = entity.Username };
            return user;
        }

         public async Task<AuthResult> RefreshTokenAsync(TokenDto tokenDto)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            if (principal == null)
            {
                return null;
            }

            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            var savedRefreshToken = await _cacheService.GetCacheAsync<string>(userId.ToString());

            if (savedRefreshToken != tokenDto.RefreshToken)
            {
                return null;
            }

            var newAccessToken = _jwtService.GenerateAccessToken(userId);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Save new refresh token in Redis
            await _cacheService.SetCacheAsync(userId.ToString(), newRefreshToken, TimeSpan.FromDays(15));

            return new AuthResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
