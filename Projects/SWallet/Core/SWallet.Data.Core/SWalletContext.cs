﻿using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Core
{
    public class SWalletContext : DbContext
    {
        public SWalletContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}