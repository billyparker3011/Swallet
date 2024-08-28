using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCasinoTicketBetDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "CasinoTicketEventDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WinOrLossAmount",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValidAmount",
                table: "CasinoTicketBetDetail",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "CasinoTicketEventDetail",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<long>(
                name: "WinOrLossAmount",
                table: "CasinoTicketBetDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ValidAmount",
                table: "CasinoTicketBetDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);
        }
    }
}
