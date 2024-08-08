using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TonWallet.Domain.Entities;

namespace TonWallet.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<bool> IsUserExist(int id);
        Task CreateUser(User user);
        Task UpdateUserWalletAddress(User user);
        Task<string> GetUserWalletAddress(int userId);
    }
}
