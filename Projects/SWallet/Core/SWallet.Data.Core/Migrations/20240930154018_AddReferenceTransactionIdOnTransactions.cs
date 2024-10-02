using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceTransactionIdOnTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReferenceTransactionId",
                table: "Transactions",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceTransactionId",
                table: "Transactions");
        }
    }
}
