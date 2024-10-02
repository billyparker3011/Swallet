using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WithdrawBankName",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawCardHolder",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawNumberAccount",
                table: "Transactions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WithdrawPaymentMethodId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WithdrawPaymentPartnerId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawToBankName",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawToCardHolder",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawToNumberAccount",
                table: "Transactions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithdrawBankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawCardHolder",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawNumberAccount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawPaymentMethodId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawPaymentPartnerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawToBankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawToCardHolder",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WithdrawToNumberAccount",
                table: "Transactions");
        }
    }
}
