using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDateTimeOffSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DateTimeOffSet",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTimeOffSet",
                table: "Settings");
        }
    }
}
