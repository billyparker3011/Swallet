using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitSwalletPartnerTicketDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CasinoTicket",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(type: "bigint", nullable: false),
                    BookiePlayerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRetry = table.Column<bool>(type: "bit", nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    CancelTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRetryCancel = table.Column<bool>(type: "bit", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    SupermasterId = table.Column<long>(type: "bigint", nullable: false),
                    WinlossAmountTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTicket_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CockFightTickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    SupermasterId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    AnteAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ArenaCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FightNumber = table.Column<int>(type: "int", nullable: false),
                    MatchDayCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Odds = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Selection = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SettledDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TicketAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    WinlossAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TicketCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidStake = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ShowMore = table.Column<bool>(type: "bit", nullable: true),
                    OddsType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                name: "M8xsTickets",
                columns: table => new
                {
                    TicketId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    SupermasterId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    SportKindId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    KickOffTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    ChoosenNumbers = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ShowMore = table.Column<bool>(type: "bit", nullable: true),
                    RewardRate = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Stake = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CustomerOdds = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    CustomerPayout = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CustomerWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DraftCustomerWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    Prize = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    Times = table.Column<int>(type: "int", nullable: true),
                    MixedTimes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CorrelationCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M8xsTickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_M8xsTickets_M8xsTickets_ParentId",
                        column: x => x.ParentId,
                        principalTable: "M8xsTickets",
                        principalColumn: "TicketId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket",
                column: "BookiePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_CustomerId",
                table: "CasinoTicket",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_TransactionId",
                table: "CasinoTicket",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_AgentId",
                table: "CockFightTickets",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_BetKindId",
                table: "CockFightTickets",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_CustomerId",
                table: "CockFightTickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_MasterId",
                table: "CockFightTickets",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_Sid",
                table: "CockFightTickets",
                column: "Sid");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_SupermasterId",
                table: "CockFightTickets",
                column: "SupermasterId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_AgentId",
                table: "M8xsTickets",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_BetKindId",
                table: "M8xsTickets",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_CustomerId",
                table: "M8xsTickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_MasterId",
                table: "M8xsTickets",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_ParentId",
                table: "M8xsTickets",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_SportKindId",
                table: "M8xsTickets",
                column: "SportKindId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsTickets_SupermasterId",
                table: "M8xsTickets",
                column: "SupermasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoTicket");

            migrationBuilder.DropTable(
                name: "CockFightTickets");

            migrationBuilder.DropTable(
                name: "M8xsTickets");
        }
    }
}
