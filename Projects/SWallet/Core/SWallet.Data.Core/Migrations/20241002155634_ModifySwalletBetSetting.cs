using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class ModifySwalletBetSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_M8xsAgentBetSettings_Managers_ManagerId",
                table: "M8xsAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CasinoAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CockFightAgentBetSettings");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "M8xsAgentBetSettings",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_M8xsAgentBetSettings_ManagerId",
                table: "M8xsAgentBetSettings",
                newName: "IX_M8xsAgentBetSettings_CustomerId");

            migrationBuilder.CreateTable(
                name: "CasinoCustomerBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CasinoCustomerBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoCustomerBetSettings_CasinoBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CasinoBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasinoCustomerBetSettings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CockFightCustomerBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CockFightCustomerBetSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CockFightCustomerBetSettings_CockFightBetKinds_BetKindId",
                        column: x => x.BetKindId,
                        principalTable: "CockFightBetKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CockFightCustomerBetSettings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoCustomerBetSettings_BetKindId",
                table: "CasinoCustomerBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoCustomerBetSettings_CustomerId",
                table: "CasinoCustomerBetSettings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightCustomerBetSettings_BetKindId",
                table: "CockFightCustomerBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightCustomerBetSettings_CustomerId",
                table: "CockFightCustomerBetSettings",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_M8xsAgentBetSettings_Customers_CustomerId",
                table: "M8xsAgentBetSettings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_M8xsAgentBetSettings_Customers_CustomerId",
                table: "M8xsAgentBetSettings");

            migrationBuilder.DropTable(
                name: "CasinoCustomerBetSettings");

            migrationBuilder.DropTable(
                name: "CockFightCustomerBetSettings");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "M8xsAgentBetSettings",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_M8xsAgentBetSettings_CustomerId",
                table: "M8xsAgentBetSettings",
                newName: "IX_M8xsAgentBetSettings_ManagerId");

            migrationBuilder.CreateTable(
                name: "CasinoAgentBetSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    DefaultVipHandicapId = table.Column<int>(type: "int", nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxLose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxWin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
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
                    BetKindId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    DrawLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    LimitNumTicketPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MainLimitAmountPerFight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettings_BetKindId",
                table: "CasinoAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettings_ManagerId",
                table: "CasinoAgentBetSettings",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_BetKindId",
                table: "CockFightAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_ManagerId",
                table: "CockFightAgentBetSettings",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_M8xsAgentBetSettings_Managers_ManagerId",
                table: "M8xsAgentBetSettings",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
