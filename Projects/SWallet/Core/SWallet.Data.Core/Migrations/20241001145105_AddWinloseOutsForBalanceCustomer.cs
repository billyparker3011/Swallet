using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddWinloseOutsForBalanceCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HistoryTotalWinlose",
                table: "BalanceCustomers",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOutstanding",
                table: "BalanceCustomers",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWinlose",
                table: "BalanceCustomers",
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
                name: "HistoryTotalWinlose",
                table: "BalanceCustomers");

            migrationBuilder.DropColumn(
                name: "TotalOutstanding",
                table: "BalanceCustomers");

            migrationBuilder.DropColumn(
                name: "TotalWinlose",
                table: "BalanceCustomers");
        }
    }
}
