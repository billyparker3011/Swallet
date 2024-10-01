using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMainDomainForSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainDomain",
                table: "Settings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainDomain",
                table: "Settings");
        }
    }
}
