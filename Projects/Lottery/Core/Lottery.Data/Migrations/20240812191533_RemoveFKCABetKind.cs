using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFKCABetKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CABetKind_BookieId",
                table: "CABetKind");

            migrationBuilder.DropColumn(
                name: "BookieId",
                table: "CABetKind");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookieId",
                table: "CABetKind",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CABetKind_BookieId",
                table: "CABetKind",
                column: "BookieId");
        }
    }
}
