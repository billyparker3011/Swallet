using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCockFightBetKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlInitCockFightBetKinds = Path.Combine(baseScriptDirectory, "InitCockFightBetKinds.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitCockFightBetKinds));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
