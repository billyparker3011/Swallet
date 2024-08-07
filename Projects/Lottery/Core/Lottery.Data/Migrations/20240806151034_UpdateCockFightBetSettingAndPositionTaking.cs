using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCockFightBetSettingAndPositionTaking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MainLimitAmountPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimitNumTicketPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DrawLimitAmountPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PositionTaking",
                table: "CockFightAgentPostionTakings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MainLimitAmountPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimitNumTicketPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DrawLimitAmountPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightPlayerBetSettings_BetKindId",
                table: "CockFightPlayerBetSettings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightPlayerBetSettings_PlayerId",
                table: "CockFightPlayerBetSettings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentPostionTakings_AgentId",
                table: "CockFightAgentPostionTakings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentPostionTakings_BetKindId",
                table: "CockFightAgentPostionTakings",
                column: "BetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_AgentId",
                table: "CockFightAgentBetSettings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CockFightAgentBetSettings_BetKindId",
                table: "CockFightAgentBetSettings",
                column: "BetKindId");

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightAgentBetSettings_Agents_AgentId",
                table: "CockFightAgentBetSettings",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightAgentBetSettings_CockFightBetKinds_BetKindId",
                table: "CockFightAgentBetSettings",
                column: "BetKindId",
                principalTable: "CockFightBetKinds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightAgentPostionTakings_Agents_AgentId",
                table: "CockFightAgentPostionTakings",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightAgentPostionTakings_CockFightBetKinds_BetKindId",
                table: "CockFightAgentPostionTakings",
                column: "BetKindId",
                principalTable: "CockFightBetKinds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightPlayerBetSettings_CockFightBetKinds_BetKindId",
                table: "CockFightPlayerBetSettings",
                column: "BetKindId",
                principalTable: "CockFightBetKinds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CockFightPlayerBetSettings_Players_PlayerId",
                table: "CockFightPlayerBetSettings",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CockFightAgentBetSettings_Agents_AgentId",
                table: "CockFightAgentBetSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_CockFightAgentBetSettings_CockFightBetKinds_BetKindId",
                table: "CockFightAgentBetSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_CockFightAgentPostionTakings_Agents_AgentId",
                table: "CockFightAgentPostionTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_CockFightAgentPostionTakings_CockFightBetKinds_BetKindId",
                table: "CockFightAgentPostionTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_CockFightPlayerBetSettings_CockFightBetKinds_BetKindId",
                table: "CockFightPlayerBetSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_CockFightPlayerBetSettings_Players_PlayerId",
                table: "CockFightPlayerBetSettings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightPlayerBetSettings_BetKindId",
                table: "CockFightPlayerBetSettings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightPlayerBetSettings_PlayerId",
                table: "CockFightPlayerBetSettings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightAgentPostionTakings_AgentId",
                table: "CockFightAgentPostionTakings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightAgentPostionTakings_BetKindId",
                table: "CockFightAgentPostionTakings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightAgentBetSettings_AgentId",
                table: "CockFightAgentBetSettings");

            migrationBuilder.DropIndex(
                name: "IX_CockFightAgentBetSettings_BetKindId",
                table: "CockFightAgentBetSettings");

            migrationBuilder.AlterColumn<decimal>(
                name: "MainLimitAmountPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "LimitNumTicketPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "DrawLimitAmountPerFight",
                table: "CockFightPlayerBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "PositionTaking",
                table: "CockFightAgentPostionTakings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "MainLimitAmountPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "LimitNumTicketPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "DrawLimitAmountPerFight",
                table: "CockFightAgentBetSettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);
        }
    }
}
