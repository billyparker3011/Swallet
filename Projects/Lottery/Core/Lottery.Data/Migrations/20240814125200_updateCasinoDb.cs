using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateCasinoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CasinoPlayerBetSettings_CasinoPlayerMappings_PlayerMappingId",
                table: "CasinoPlayerBetSettings");

            migrationBuilder.RenameColumn(
                name: "PlayerMappingId",
                table: "CasinoPlayerBetSettings",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_CasinoPlayerBetSettings_PlayerMappingId",
                table: "CasinoPlayerBetSettings",
                newName: "IX_CasinoPlayerBetSettings_PlayerId");

            migrationBuilder.AddColumn<string>(
                name: "Caterory",
                table: "CasinoGameTypes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "CasinoBetKinds",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CasinoBetKinds",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CasinoAgentBetSettingAgentHandicaps",
                columns: table => new
                {
                    CasinoAgentBetSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CasinoAgentHandicapId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoAgentBetSettingAgentHandicaps", x => new { x.CasinoAgentBetSettingId, x.CasinoAgentHandicapId });
                    table.ForeignKey(
                        name: "FK_CasinoAgentBetSettingAgentHandicaps_CasinoAgentBetSettings_CasinoAgentBetSettingId",
                        column: x => x.CasinoAgentBetSettingId,
                        principalTable: "CasinoAgentBetSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CasinoAgentBetSettingAgentHandicaps_CasinoAgentHandicaps_CasinoAgentHandicapId",
                        column: x => x.CasinoAgentHandicapId,
                        principalTable: "CasinoAgentHandicaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CasinoPlayerBetSettingAgentHandicaps",
                columns: table => new
                {
                    CasinoPlayerBetSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CasinoAgentHandicapId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoPlayerBetSettingAgentHandicaps", x => new { x.CasinoPlayerBetSettingId, x.CasinoAgentHandicapId });
                    table.ForeignKey(
                        name: "FK_CasinoPlayerBetSettingAgentHandicaps_CasinoAgentHandicaps_CasinoAgentHandicapId",
                        column: x => x.CasinoAgentHandicapId,
                        principalTable: "CasinoAgentHandicaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasinoPlayerBetSettingAgentHandicaps_CasinoPlayerBetSettings_CasinoPlayerBetSettingId",
                        column: x => x.CasinoPlayerBetSettingId,
                        principalTable: "CasinoPlayerBetSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettingAgentHandicaps_CasinoAgentBetSettingId_CasinoAgentHandicapId",
                table: "CasinoAgentBetSettingAgentHandicaps",
                columns: new[] { "CasinoAgentBetSettingId", "CasinoAgentHandicapId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CasinoAgentBetSettingAgentHandicaps_CasinoAgentHandicapId",
                table: "CasinoAgentBetSettingAgentHandicaps",
                column: "CasinoAgentHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoPlayerBetSettingAgentHandicaps_CasinoAgentHandicapId",
                table: "CasinoPlayerBetSettingAgentHandicaps",
                column: "CasinoAgentHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoPlayerBetSettingAgentHandicaps_CasinoPlayerBetSettingId_CasinoAgentHandicapId",
                table: "CasinoPlayerBetSettingAgentHandicaps",
                columns: new[] { "CasinoPlayerBetSettingId", "CasinoAgentHandicapId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CasinoPlayerBetSettings_Players_PlayerId",
                table: "CasinoPlayerBetSettings",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CasinoPlayerBetSettings_Players_PlayerId",
                table: "CasinoPlayerBetSettings");

            migrationBuilder.DropTable(
                name: "CasinoAgentBetSettingAgentHandicaps");

            migrationBuilder.DropTable(
                name: "CasinoPlayerBetSettingAgentHandicaps");

            migrationBuilder.DropColumn(
                name: "Caterory",
                table: "CasinoGameTypes");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "CasinoPlayerBetSettings",
                newName: "PlayerMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_CasinoPlayerBetSettings_PlayerId",
                table: "CasinoPlayerBetSettings",
                newName: "IX_CasinoPlayerBetSettings_PlayerMappingId");

            migrationBuilder.AlterColumn<string>(
                name: "RegionId",
                table: "CasinoBetKinds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "CasinoBetKinds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CasinoPlayerBetSettings_CasinoPlayerMappings_PlayerMappingId",
                table: "CasinoPlayerBetSettings",
                column: "PlayerMappingId",
                principalTable: "CasinoPlayerMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
