using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TonWallet.Domain.Entities;

namespace TonWallet.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WalletAddress> WalletAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletAddress>()
                .OwnsOne(p => p.Bounceable);

            modelBuilder.Entity<WalletAddress>()
                .OwnsOne(p => p.NonBounceable);
        }
    }
}
