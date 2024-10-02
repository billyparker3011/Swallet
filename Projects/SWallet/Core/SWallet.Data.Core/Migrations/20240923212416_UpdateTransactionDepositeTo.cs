using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionDepositeTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositToBankName",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositToCardHolder",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositToNumberAccount",
                table: "Transactions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositToBankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositToCardHolder",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositToNumberAccount",
                table: "Transactions");
        }
    }
}
