using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMixedCorrelationBetKindIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "CorrelationBetKindIds",
            //    table: "BetKinds",
            //    type: "nvarchar(15)",
            //    maxLength: 15,
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsMixed",
            //    table: "BetKinds",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CorrelationBetKindIds",
            //    table: "BetKinds");

            //migrationBuilder.DropColumn(
            //    name: "IsMixed",
            //    table: "BetKinds");
        }
    }
}
