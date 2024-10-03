using Microsoft.EntityFrameworkCore;
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
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ManagerSession> ManagerSessions { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountDetail> DiscountDetails { get; set; }
        public DbSet<CustomerBankAccount> CustomerBankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BalanceCustomer> BalanceCustomers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        #region M8XS
        public DbSet<M8xsBetKind> M8XsBetKinds { get; set; }
        public DbSet<M8xsCustomerBetSetting> M8XsAgentBetSettings { get; set; }
        public DbSet<M8xsTicket> M8xsTickets { get; set; }
        #endregion
        #region CockFight
        public DbSet<CockFightBetKind> CockFightBetKinds { get; set; }
        public DbSet<CockFightCustomerBetSetting> CockFightAgentBetSettings { get; set; }
        public DbSet<CockFightTicket> CockFightTickets { get; set; }
        #endregion
        #region Casino
        public DbSet<CasinoBetKind> CasinoBetKinds { get; set; }
        public DbSet<CasinoCustomerBetSetting> CasinoAgentBetSettings { get; set; }
        public DbSet<CasinoTicket> CasinoTickets { get; set; }
        #endregion
    }
}
