using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWinlossAllLevelOfTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgentPt",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AgentWinLoss",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyWinLoss",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterPt",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterWinLoss",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterPt",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterWinLoss",
                table: "CockFightTickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentPt",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "AgentWinLoss",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "CompanyWinLoss",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "MasterPt",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "MasterWinLoss",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "SupermasterPt",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "SupermasterWinLoss",
                table: "CockFightTickets");
        }
    }
}
