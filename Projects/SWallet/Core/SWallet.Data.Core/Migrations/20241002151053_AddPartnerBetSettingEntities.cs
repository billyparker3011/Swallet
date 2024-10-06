using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerBetSettingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CasinoBetKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    IsLive = table.Column<bool>(type: "bit", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoBetKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CockFightBetKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockFightBetKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "M8xsBetKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    ReplaceByIdWhenLive = table.Column<int>(type: "int", nullable: true),
                    OrderInCategory = table.Column<int>(type: "int", nullable: false),
                    Award = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    IsMixed = table.Column<bool>(type: "bit", nullable: false),
                    CorrelationBetKindIds = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M8xsBetKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CasinoAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    DefaultVipHandicapId = table.Column<int>(type: "int", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxWin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxLose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoAgentBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoAgentBetSettings_CasinoBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CasinoBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasinoAgentBetSettings_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "ManagerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CockFightAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<long>(type: "bigint", nullable: false),
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
                        name: "FK_CockFightAgentBetSettings_CockFightBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CockFightBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CockFightAgentBetSettings_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "ManagerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "M8xsAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<long>(type: "bigint", nullable: false),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    Buy = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MinBuy = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxBuy = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MaxPerNumber = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M8xsAgentBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_M8xsAgentBetSettings_M8xsBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "M8xsBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_M8xsAgentBetSettings_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "ManagerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettings_BetKindId",
                table: "CasinoAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettings_ManagerId",
                table: "CasinoAgentBetSettings",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoBetKinds_Code",
                table: "CasinoBetKinds",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_BetKindId",
                table: "CockFightAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_ManagerId",
                table: "CockFightAgentBetSettings",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsAgentBetSettings_BetKindId",
                table: "M8xsAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_M8xsAgentBetSettings_ManagerId",
                table: "M8xsAgentBetSettings",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CockFightAgentBetSettings");

            migrationBuilder.DropTable(
                name: "M8xsAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CasinoBetKinds");

            migrationBuilder.DropTable(
                name: "CockFightBetKinds");

            migrationBuilder.DropTable(
                name: "M8xsBetKinds");
        }
    }
}
