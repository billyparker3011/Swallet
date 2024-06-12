using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketTableForWinloseDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupermasterOdd",
                table: "Tickets",
                newName: "SupermasterOdds");

            migrationBuilder.RenameColumn(
                name: "PlayerOdd",
                table: "Tickets",
                newName: "PlayerOdds");

            migrationBuilder.RenameColumn(
                name: "MasterOdd",
                table: "Tickets",
                newName: "MasterOdds");

            migrationBuilder.RenameColumn(
                name: "CompanyOdd",
                table: "Tickets",
                newName: "CompanyOdds");

            migrationBuilder.RenameColumn(
                name: "AgentOdd",
                table: "Tickets",
                newName: "AgentOdds");

            migrationBuilder.AddColumn<decimal>(
                name: "AgentCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AgentPt",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MasterPt",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SupermasterPt",
                table: "Tickets",
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
                name: "AgentCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AgentPt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MasterCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MasterPt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SupermasterCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SupermasterPt",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "SupermasterOdds",
                table: "Tickets",
                newName: "SupermasterOdd");

            migrationBuilder.RenameColumn(
                name: "PlayerOdds",
                table: "Tickets",
                newName: "PlayerOdd");

            migrationBuilder.RenameColumn(
                name: "MasterOdds",
                table: "Tickets",
                newName: "MasterOdd");

            migrationBuilder.RenameColumn(
                name: "CompanyOdds",
                table: "Tickets",
                newName: "CompanyOdd");

            migrationBuilder.RenameColumn(
                name: "AgentOdds",
                table: "Tickets",
                newName: "AgentOdd");
        }
    }
}
