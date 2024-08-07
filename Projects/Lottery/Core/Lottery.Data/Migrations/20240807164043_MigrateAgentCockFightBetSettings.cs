using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAgentCockFightBetSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlMigrateAgentCockFightBetSettings = Path.Combine(baseScriptDirectory, "MigrateAgentCockFightBetSettings.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlMigrateAgentCockFightBetSettings));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
