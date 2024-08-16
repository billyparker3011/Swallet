using Lottery.Data.Entities;
using Lottery.Data.Entities.Partners.Casino;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Data
{
    public class LotteryContext : DbContext
    {
        public LotteryContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<BetKind> BetKinds { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<MatchResult> MatchResults { get; set; }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentSession> AgentSessions { get; set; }
        public virtual DbSet<AgentAudit> AgentAudits { get; set; }
        public virtual DbSet<AgentOdd> AgentOdds { get; set; }
        public virtual DbSet<AgentPositionTaking> AgentPositionTakings { get; set; }

        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<PlayerSession> PlayerSessions { get; set; }
        public virtual DbSet<PlayerAudit> PlayerAudits { get; set; }
        public virtual DbSet<PlayerOdd> PlayerOdds { get; set; }

        public virtual DbSet<Ticket> Tickets { get; set; }

        public virtual DbSet<Audit> Audits { get; set; }

        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<AgentAnnouncement> AgentAnnouncements { get; set; }
        public virtual DbSet<PlayerAnnouncement> PlayerAnnouncements { get; set; }

        public virtual DbSet<Setting> Settings { get; set; }

        public virtual DbSet<BookieSetting> BookieSettings { get; set; }

        #region Cock Fight Partner
        public virtual DbSet<CockFightBetKind> CockFightBetKinds { get; set; }
        public virtual DbSet<CockFightAgentBetSetting> CockFightAgentBetSettings { get; set; }
        public virtual DbSet<CockFightAgentPostionTaking> CockFightAgentPostionTakings { get; set; }
        public virtual DbSet<CockFightPlayerBetSetting> CockFightPlayerBetSettings { get; set; }
        public virtual DbSet<CockFightPlayerMapping> CockFightPlayerMappings { get; set; }
        public virtual DbSet<CockFightTicket> CockFightTickets { get; set; }
        #endregion

        #region Casino Partner
        public virtual DbSet<CasinoBetKind> CasinoBetKinds { get; set; }
        public virtual DbSet<CasinoGameType> CasinoGameTypes { get; set; }
        public virtual DbSet<CasinoAgentHandicap> CasinoAgentHandicaps { get; set; }
        public virtual DbSet<CasinoAgentBetSetting> CasinoAgentBetSettings { get; set; }
        public virtual DbSet<CasinoAgentBetSettingAgentHandicap> CasinoAgentBetSettingAgentHandicaps { get; set; }
        public virtual DbSet<CasinoAgentPositionTaking> CasinoAgentPositionTakings { get; set; }
        public virtual DbSet<CasinoPlayerBetSetting> CasinoPlayerBetSettings { get; set; }
        public virtual DbSet<CasinoPlayerBetSettingAgentHandicap> CasinoPlayerBetSettingAgentHandicaps { get; set; }
        public virtual DbSet<CasinoPlayerMapping> CasinoPlayerMappings { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Entity<CasinoAgentBetSettingAgentHandicap>()
.HasKey(ca => new { ca.CasinoAgentBetSettingId, ca.CasinoAgentHandicapId });

            modelBuilder.Entity<CasinoAgentBetSettingAgentHandicap>()
                .HasOne(ca => ca.CasinoAgentBetSetting)
                .WithMany(s => s.CasinoAgentBetSettingAgentHandicaps)
                .HasForeignKey(ca => ca.CasinoAgentBetSettingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CasinoAgentBetSettingAgentHandicap>()
                .HasOne(sc => sc.CasinoAgentHandicap)
                .WithMany()
                .HasForeignKey(sc => sc.CasinoAgentHandicapId);

            modelBuilder.Entity<CasinoPlayerBetSettingAgentHandicap>()
                .HasKey(ca => new { ca.CasinoPlayerBetSettingId, ca.CasinoAgentHandicapId });

            modelBuilder.Entity<CasinoPlayerBetSettingAgentHandicap>()
               .HasOne(ca => ca.CasinoPlayerBetSetting)
               .WithMany(s => s.CasinoPlayerBetSettingAgentHandicaps)
               .HasForeignKey(ca => ca.CasinoPlayerBetSettingId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CasinoPlayerBetSettingAgentHandicap>()
                .HasOne(sc => sc.CasinoAgentHandicap)
                .WithMany()
                .HasForeignKey(sc => sc.CasinoAgentHandicapId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
