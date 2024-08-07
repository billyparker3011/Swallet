using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigratePlayerCockFightBetSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlMigratePlayerCockFightBetSettings = Path.Combine(baseScriptDirectory, "MigratePlayerCockFightBetSettings.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlMigratePlayerCockFightBetSettings));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
