using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRefBalanceCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BalanceCustomers_CustomerId",
                table: "BalanceCustomers");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceCustomers_CustomerId",
                table: "BalanceCustomers",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BalanceCustomers_CustomerId",
                table: "BalanceCustomers");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceCustomers_CustomerId",
                table: "BalanceCustomers",
                column: "CustomerId");
        }
    }
}
