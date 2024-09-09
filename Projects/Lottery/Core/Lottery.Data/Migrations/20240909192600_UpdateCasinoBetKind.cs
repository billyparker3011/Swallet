using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCasinoBetKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Award",
                table: "CasinoBetKinds");

            migrationBuilder.DropColumn(
                name: "OrderInCategory",
                table: "CasinoBetKinds");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "CasinoBetKinds");

            migrationBuilder.RenameColumn(
                name: "Caterory",
                table: "CasinoGameTypes",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "CasinoGameTypes",
                newName: "Caterory");

            migrationBuilder.AddColumn<decimal>(
                name: "Award",
                table: "CasinoBetKinds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderInCategory",
                table: "CasinoBetKinds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "CasinoBetKinds",
                type: "int",
                nullable: true);
        }
    }
}
