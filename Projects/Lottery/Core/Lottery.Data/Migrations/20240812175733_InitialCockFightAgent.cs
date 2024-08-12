﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCockFightAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CockFightBetKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightBetKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CockFightPlayerMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    IsInitial = table.Column<bool>(type: "bit", nullable: false),
                    MemberRefId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFreeze = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightPlayerMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CockFightTickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    SupermasterId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberRefId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AnteAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ArenaCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KickOffDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FightNumber = table.Column<int>(type: "int", nullable: false),
                    MatchDayCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Odds = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Selection = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SettledDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TicketAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WinlossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CockFightAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    MainLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DrawLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    LimitNumTicketPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightAgentBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CockFightAgentBetSettings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CockFightAgentBetSettings_CockFightBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CockFightBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CockFightAgentPostionTakings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    PositionTaking = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightAgentPostionTakings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CockFightAgentPostionTakings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CockFightAgentPostionTakings_CockFightBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CockFightBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CockFightPlayerBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    MainLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DrawLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    LimitNumTicketPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightPlayerBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CockFightPlayerBetSettings_CockFightBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CockFightBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CockFightPlayerBetSettings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_AgentId",
                table: "CockFightAgentBetSettings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_BetKindId",
                table: "CockFightAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentPostionTakings_AgentId",
                table: "CockFightAgentPostionTakings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentPostionTakings_BetKindId",
                table: "CockFightAgentPostionTakings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightPlayerBetSettings_BetKindId",
                table: "CockFightPlayerBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightPlayerBetSettings_PlayerId",
                table: "CockFightPlayerBetSettings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_AgentId",
                table: "CockFightTickets",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_BetKindId",
                table: "CockFightTickets",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_MasterId",
                table: "CockFightTickets",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_PlayerId",
                table: "CockFightTickets",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_SupermasterId",
                table: "CockFightTickets",
                column: "SupermasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CockFightAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CockFightAgentPostionTakings");

            migrationBuilder.DropTable(
                name: "CockFightPlayerBetSettings");

            migrationBuilder.DropTable(
                name: "CockFightPlayerMappings");

            migrationBuilder.DropTable(
                name: "CockFightTickets");

            migrationBuilder.DropTable(
                name: "CockFightBetKinds");
        }
    }
}
