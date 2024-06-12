using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class _11062024_UpdateAuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Audits",
                newName: "SupermasterId");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Audits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgentId",
                table: "Audits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "EdittedBy",
                table: "Audits",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Audits",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Audits",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "Audits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Audits",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_UserName",
                table: "Audits",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Audits_UserName",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "EdittedBy",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "SupermasterId",
                table: "Audits",
                newName: "CreatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Audits",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Audits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "Audits",
                type: "bigint",
                nullable: true);
        }
    }
}
