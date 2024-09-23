using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositBankName",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositCardHolder",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositContent",
                table: "Transactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositNumberAccount",
                table: "Transactions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepositPaymentMethodId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepositPaymentPartnerId",
                table: "Transactions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositBankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositCardHolder",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositContent",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositNumberAccount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositPaymentMethodId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DepositPaymentPartnerId",
                table: "Transactions");
        }
    }
}
