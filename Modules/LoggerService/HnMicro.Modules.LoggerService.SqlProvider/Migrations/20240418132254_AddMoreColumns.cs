using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HnMicro.Modules.LoggerService.SqlProvider.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Logs",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Logs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Logs");
        }
    }
}
