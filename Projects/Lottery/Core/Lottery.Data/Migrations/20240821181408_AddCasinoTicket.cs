using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCasinoTicket : Migration
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
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTicket_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CasinoTicketBetDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BetNum = table.Column<long>(type: "bigint", nullable: false),
                    GameRoundId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Deposit = table.Column<long>(type: "bigint", nullable: false),
                    GameType = table.Column<int>(type: "int", nullable: false),
                    BetType = table.Column<int>(type: "int", nullable: false),
                    Commission = table.Column<int>(type: "int", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GameResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GameResult2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WinOrLossAmount = table.Column<long>(type: "bigint", nullable: true),
                    ValidAmount = table.Column<long>(type: "bigint", nullable: true),
                    BetTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BetMethod = table.Column<long>(type: "bigint", nullable: false),
                    AppType = table.Column<long>(type: "bigint", nullable: false),
                    GameRoundStartTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GameRoundEndTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    CasinoTicketId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTicketBetDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTicketBetDetail_CasinoTicket_CasinoTicketId",
                        column: x => x.CasinoTicketId,
                        principalTable: "CasinoTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CasinoTicketEventDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    EventCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EventRecordNum = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRate = table.Column<long>(type: "bigint", nullable: false),
                    SettleTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CasinoTicketId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTicketEventDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTicketEventDetail_CasinoTicket_CasinoTicketId",
                        column: x => x.CasinoTicketId,
                        principalTable: "CasinoTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CockFightTickets_Sid",
                table: "CockFightTickets",
                column: "Sid");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket",
                column: "BookiePlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_PlayerId",
                table: "CasinoTicket",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_TransactionId",
                table: "CasinoTicket",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicketBetDetail_CasinoTicketId",
                table: "CasinoTicketBetDetail",
                column: "CasinoTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicketEventDetail_CasinoTicketId",
                table: "CasinoTicketEventDetail",
                column: "CasinoTicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoTicketBetDetail");

            migrationBuilder.DropTable(
                name: "CasinoTicketEventDetail");

            migrationBuilder.DropTable(
                name: "CasinoTicket");

            migrationBuilder.DropIndex(
                name: "IX_CockFightTickets_Sid",
                table: "CockFightTickets");
        }
    }
}
