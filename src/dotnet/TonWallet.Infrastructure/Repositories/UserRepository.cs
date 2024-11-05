using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TonWallet.Domain.Entities;
using TonWallet.Domain.Repositories;

namespace TonWallet.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateUser(User user)
        {
            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetUserWalletAddress(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.WalletAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user.WalletAddresses.RawForm;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users
                .Include(u => u.WalletAddresses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsUserExist(int id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }

        public async Task UpdateUserWalletAddress(User user)
        {
            var entity = await _dbContext.Users
                .Include(u => u.WalletAddresses)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            entity.WalletAddresses.RawForm = user.WalletAddresses.RawForm;
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var entity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
