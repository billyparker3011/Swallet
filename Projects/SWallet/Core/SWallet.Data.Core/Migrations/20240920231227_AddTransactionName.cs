using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionName",
                table: "Transactions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionName",
                table: "Transactions");
        }
    }
}
