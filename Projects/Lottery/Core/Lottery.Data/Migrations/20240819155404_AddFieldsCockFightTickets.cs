using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsCockFightTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "CockFightTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMore",
                table: "CockFightTickets",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CockFightTickets");

            migrationBuilder.DropColumn(
                name: "ShowMore",
                table: "CockFightTickets");
        }
    }
}
