using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCasinoTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket",
                column: "BookiePlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTicket_BookiePlayerId",
                table: "CasinoTicket",
                column: "BookiePlayerId",
                unique: true);
        }
    }
}
