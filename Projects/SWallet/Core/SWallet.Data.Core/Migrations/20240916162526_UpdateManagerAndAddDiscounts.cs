using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateManagerAndAddDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerCode",
                table: "Managers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Setting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Managers_MasterId",
                table: "Managers",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_SupermasterId",
                table: "Managers",
                column: "SupermasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_Username_ManagerCode",
                table: "Managers",
                columns: new[] { "Username", "ManagerCode" },
                unique: true,
                filter: "[ManagerCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Managers_MasterId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_SupermasterId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_Username_ManagerCode",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "ManagerCode",
                table: "Managers");
        }
    }
}
