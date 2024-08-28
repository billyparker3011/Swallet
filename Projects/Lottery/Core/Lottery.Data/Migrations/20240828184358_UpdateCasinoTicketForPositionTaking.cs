using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCasinoTicketForPositionTaking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgentWinLoss",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyWinLoss",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterWinLoss",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterWinLoss",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "AgentId",
                table: "CasinoTicket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "AgentPt",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AgentWinLoss",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BetKindId",
                table: "CasinoTicket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyWinLoss",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "CasinoTicket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterPt",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterWinLoss",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "SupermasterId",
                table: "CasinoTicket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterPt",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterWinLoss",
                table: "CasinoTicket",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WinlossAmountTotal",
                table: "CasinoTicket",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentWinLoss",
                table: "CasinoTicketBetDetail");

            migrationBuilder.DropColumn(
                name: "CompanyWinLoss",
                table: "CasinoTicketBetDetail");

            migrationBuilder.DropColumn(
                name: "MasterWinLoss",
                table: "CasinoTicketBetDetail");

            migrationBuilder.DropColumn(
                name: "SupermasterWinLoss",
                table: "CasinoTicketBetDetail");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "AgentPt",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "AgentWinLoss",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "BetKindId",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "CompanyWinLoss",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "MasterPt",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "MasterWinLoss",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "SupermasterId",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "SupermasterPt",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "SupermasterWinLoss",
                table: "CasinoTicket");

            migrationBuilder.DropColumn(
                name: "WinlossAmountTotal",
                table: "CasinoTicket");
        }
    }
}
