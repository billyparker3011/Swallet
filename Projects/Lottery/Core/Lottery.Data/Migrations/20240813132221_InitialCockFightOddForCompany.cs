using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCockFightOddForCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlInitCockFightOddsForCompany = Path.Combine(baseScriptDirectory, "CreateCockFightOddForCompany.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitCockFightOddsForCompany));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
