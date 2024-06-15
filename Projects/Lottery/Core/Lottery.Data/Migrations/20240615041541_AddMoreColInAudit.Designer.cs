﻿// <auto-generated />
using System;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lottery.Data.Migrations
{
    [DbContext(typeof(LotteryContext))]
    [Migration("20240615041541_AddMoreColInAudit")]
    partial class AddMoreColInAudit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Lottery.Data.Entities.Agent", b =>
                {
                    b.Property<long>("AgentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AgentId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Credit")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("LastName")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<DateTime?>("LatestChangePassword")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LatestChangeSecurityCode")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Lock")
                        .HasColumnType("bit");

                    b.Property<long>("MasterId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("MemberMaxCredit")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ParentState")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Permissions")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityCode")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<long>("SupermasterId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("AgentId");

                    b.HasIndex("MasterId");

                    b.HasIndex("ParentId");

                    b.HasIndex("SupermasterId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentAudit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Headers")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Platform")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.ToTable("AgentAudits");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentOdd", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<int>("BetKindId")
                        .HasColumnType("int");

                    b.Property<decimal>("Buy")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<decimal>("MaxBet")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MaxBuy")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MaxPerNumber")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MinBet")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MinBuy")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("BetKindId");

                    b.ToTable("AgentOdds");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentPositionTaking", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<int>("BetKindId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<decimal>("PositionTaking")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("BetKindId");

                    b.ToTable("AgentPositionTakings");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Hash")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("LatestDoingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Platform")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("AgentId")
                        .IsUnique();

                    b.ToTable("AgentSessions");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Audit", b =>
                {
                    b.Property<long>("AuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AuditId"));

                    b.Property<string>("Action")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<string>("AuditData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuditSettingDatas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EdittedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("LastName")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<long>("MasterId")
                        .HasColumnType("bigint");

                    b.Property<long>("SupermasterId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("AuditId");

                    b.HasIndex("UserName");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("Lottery.Data.Entities.BetKind", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<decimal>("Award")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("CorrelationBetKindIds")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMixed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("OrderInCategory")
                        .HasColumnType("int");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<int?>("ReplaceByIdWhenLive")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("BetKinds");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DayOfWeeks")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Match", b =>
                {
                    b.Property<long>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("MatchId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("KickOffTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("MatchCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("MatchState")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("MatchId");

                    b.HasIndex("KickOffTime");

                    b.HasIndex("MatchCode")
                        .IsUnique();

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Lottery.Data.Entities.MatchResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<bool>("EnabledProcessTicket")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLive")
                        .HasColumnType("bit");

                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<string>("Results")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MatchId", "RegionId", "ChannelId");

                    b.ToTable("MatchResults");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Player", b =>
                {
                    b.Property<long>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PlayerId"));

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Credit")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("LastName")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("LatestChangePassword")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Lock")
                        .HasColumnType("bit");

                    b.Property<long>("MasterId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ParentState")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<long>("SupermasterId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("PlayerId");

                    b.HasIndex("AgentId");

                    b.HasIndex("MasterId");

                    b.HasIndex("SupermasterId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerAudit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Headers")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Platform")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerAudits");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerOdd", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("BetKindId")
                        .HasColumnType("int");

                    b.Property<decimal>("Buy")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<decimal>("MaxBet")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MaxPerNumber")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MinBet")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("BetKindId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerOdds");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Hash")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("LatestDoingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Platform")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId")
                        .IsUnique();

                    b.ToTable("PlayerSessions");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Prize", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("NoOfNumbers")
                        .HasColumnType("int");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("PrizeId")
                        .HasColumnType("int");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PrizeId", "RegionId")
                        .IsUnique();

                    b.ToTable("Prizes");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Ticket", b =>
                {
                    b.Property<long>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("TicketId"));

                    b.Property<decimal>("AgentCommission")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("AgentOdds")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("AgentPayout")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("AgentPt")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("AgentWinLoss")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("BetKindId")
                        .HasColumnType("int");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<string>("ChoosenNumbers")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal?>("CompanyOdds")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("CompanyPayout")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("CompanyWinLoss")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<Guid?>("CorrelationCode")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsLive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("KickOffTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("MasterCommission")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("MasterId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("MasterOdds")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MasterPayout")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MasterPt")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("MasterWinLoss")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<string>("MixedTimes")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Platform")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("PlayerOdds")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("PlayerPayout")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("PlayerWinLoss")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<int?>("Position")
                        .HasColumnType("int");

                    b.Property<int?>("Prize")
                        .HasColumnType("int");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<decimal?>("RewardRate")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<bool?>("ShowMore")
                        .HasColumnType("bit");

                    b.Property<int>("SportKindId")
                        .HasColumnType("int");

                    b.Property<decimal>("Stake")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<decimal>("SupermasterCommission")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<long>("SupermasterId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("SupermasterOdds")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("SupermasterPayout")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("SupermasterPt")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("SupermasterWinLoss")
                        .HasPrecision(18, 3)
                        .HasColumnType("decimal(18,3)");

                    b.Property<int?>("Times")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("TicketId");

                    b.HasIndex("AgentId");

                    b.HasIndex("BetKindId");

                    b.HasIndex("MasterId");

                    b.HasIndex("ParentId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("SportKindId");

                    b.HasIndex("SupermasterId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentAudit", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Agent", "Agent")
                        .WithMany("AgentAudits")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentOdd", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Agent", "Agent")
                        .WithMany("AgentOdds")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lottery.Data.Entities.BetKind", "BetKind")
                        .WithMany()
                        .HasForeignKey("BetKindId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");

                    b.Navigation("BetKind");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentPositionTaking", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Agent", "Agent")
                        .WithMany("AgentPositionTakings")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lottery.Data.Entities.BetKind", "BetKind")
                        .WithMany()
                        .HasForeignKey("BetKindId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");

                    b.Navigation("BetKind");
                });

            modelBuilder.Entity("Lottery.Data.Entities.AgentSession", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Agent", "Agent")
                        .WithOne("AgentSession")
                        .HasForeignKey("Lottery.Data.Entities.AgentSession", "AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");
                });

            modelBuilder.Entity("Lottery.Data.Entities.MatchResult", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Match", "Match")
                        .WithMany("MatchResults")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerAudit", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Player", "Player")
                        .WithMany("PlayerAudits")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerOdd", b =>
                {
                    b.HasOne("Lottery.Data.Entities.BetKind", "BetKind")
                        .WithMany()
                        .HasForeignKey("BetKindId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lottery.Data.Entities.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BetKind");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Lottery.Data.Entities.PlayerSession", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Player", "Player")
                        .WithOne("PlayerSession")
                        .HasForeignKey("Lottery.Data.Entities.PlayerSession", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Ticket", b =>
                {
                    b.HasOne("Lottery.Data.Entities.Ticket", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Agent", b =>
                {
                    b.Navigation("AgentAudits");

                    b.Navigation("AgentOdds");

                    b.Navigation("AgentPositionTakings");

                    b.Navigation("AgentSession");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Match", b =>
                {
                    b.Navigation("MatchResults");
                });

            modelBuilder.Entity("Lottery.Data.Entities.Player", b =>
                {
                    b.Navigation("PlayerAudits");

                    b.Navigation("PlayerSession");
                });
#pragma warning restore 612, 618
        }
    }
}
