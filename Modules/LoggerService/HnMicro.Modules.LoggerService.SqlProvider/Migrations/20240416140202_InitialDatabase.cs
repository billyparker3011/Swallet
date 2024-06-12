using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HnMicro.Modules.LoggerService.SqlProvider.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Stacktrace = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CreatedBy",
                table: "Logs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RoleId",
                table: "Logs",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ServiceCode",
                table: "Logs",
                column: "ServiceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ServiceName",
                table: "Logs",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
