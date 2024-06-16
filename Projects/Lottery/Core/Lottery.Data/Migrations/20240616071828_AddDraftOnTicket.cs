using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDraftOnTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DraftAgentCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftAgentWinLoss",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftCompanyWinLoss",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftMasterCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftMasterWinLoss",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftPlayerWinLoss",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftSupermasterCommission",
                table: "Tickets",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DraftSupermasterWinLoss",
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
                name: "DraftAgentCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftAgentWinLoss",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftCompanyWinLoss",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftMasterCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftMasterWinLoss",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftPlayerWinLoss",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftSupermasterCommission",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DraftSupermasterWinLoss",
                table: "Tickets");
        }
    }
}
