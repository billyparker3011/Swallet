using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBtiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BtiBetKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtiBetKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BtiPlayerMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerLogin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BtiAgentId = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtiPlayerMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BtiPlayerMappings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BtiTickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReserveId = table.Column<long>(type: "bigint", nullable: true),
                    RequestId = table.Column<long>(type: "bigint", nullable: true),
                    PurcahseId = table.Column<long>(type: "bigint", nullable: true),
                    IsFreeBet = table.Column<bool>(type: "bit", nullable: false),
                    TransactionId = table.Column<long>(type: "bigint", nullable: true),
                    BalanceResponse = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    StatusResponse = table.Column<int>(type: "int", nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    SupermasterId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Odds = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TicketAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    WinlossAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TicketCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidStake = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ShowMore = table.Column<bool>(type: "bit", nullable: true),
                    AgentWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    AgentPt = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MasterWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MasterPt = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SupermasterWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SupermasterPt = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CompanyWinLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    OddsType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtiTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BtiAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxWin = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtiAgentBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BtiAgentBetSettings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BtiAgentBetSettings_BtiBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "BtiBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BtiAgentPositionTakings",
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
                    table.PrimaryKey("PK_BtiAgentPositionTakings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BtiAgentPositionTakings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BtiAgentPositionTakings_BtiBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "BtiBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BtiPlayerBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxWin = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxLoss = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    IsSynchronized = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtiPlayerBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BtiPlayerBetSettings_BtiBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "BtiBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BtiPlayerBetSettings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BtiAgentBetSettings_AgentId",
                table: "BtiAgentBetSettings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiAgentBetSettings_BetKindId",
                table: "BtiAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiAgentPositionTakings_AgentId",
                table: "BtiAgentPositionTakings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiAgentPositionTakings_BetKindId",
                table: "BtiAgentPositionTakings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiBetKinds_BranchId",
                table: "BtiBetKinds",
                column: "BranchId",
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BtiPlayerBetSettings_BetKindId",
                table: "BtiPlayerBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiPlayerBetSettings_PlayerId",
                table: "BtiPlayerBetSettings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiPlayerMappings_CustomerId_CustomerLogin",
                table: "BtiPlayerMappings",
                columns: new[] { "CustomerId", "CustomerLogin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BtiPlayerMappings_PlayerId",
                table: "BtiPlayerMappings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiTickets_AgentId",
                table: "BtiTickets",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiTickets_BetKindId",
                table: "BtiTickets",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiTickets_MasterId",
                table: "BtiTickets",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiTickets_PlayerId",
                table: "BtiTickets",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_BtiTickets_SupermasterId",
                table: "BtiTickets",
                column: "SupermasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BtiAgentBetSettings");

            migrationBuilder.DropTable(
                name: "BtiAgentPositionTakings");

            migrationBuilder.DropTable(
                name: "BtiPlayerBetSettings");

            migrationBuilder.DropTable(
                name: "BtiPlayerMappings");

            migrationBuilder.DropTable(
                name: "BtiTickets");

            migrationBuilder.DropTable(
                name: "BtiBetKinds");
        }
    }
}
