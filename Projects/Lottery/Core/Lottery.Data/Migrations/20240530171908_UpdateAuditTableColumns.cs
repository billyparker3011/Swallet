using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditTableColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "Audits");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Audits",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditData",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "AuditData",
                table: "Audits");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "Audits",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "Audits",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
