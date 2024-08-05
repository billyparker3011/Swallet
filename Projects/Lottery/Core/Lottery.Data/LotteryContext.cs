using Lottery.Data.Entities;
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
        public virtual DbSet<CockFightAgentBetSetting> CockFightAgentBetSettings { get; set; }
        public virtual DbSet<CockFightAgentPostionTaking> CockFightAgentPostionTakings { get; set; }
        public virtual DbSet<CockFightBetKind> CockFightBetKinds { get; set; }
        public virtual DbSet<CockFightPlayerBetSetting> CockFightPlayerBetSettings { get; set; }
        public virtual DbSet<CockFightPlayerMapping> CockFightPlayerMappings { get; set; }
        public virtual DbSet<CockFightTicket> CockFightTickets { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
