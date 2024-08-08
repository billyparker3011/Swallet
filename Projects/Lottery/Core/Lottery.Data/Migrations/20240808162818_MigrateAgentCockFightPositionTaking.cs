using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAgentCockFightPositionTaking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlMigrateAgentCockFightPositionTakings = Path.Combine(baseScriptDirectory, "MigrateAgentCockFightPositionTakings.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlMigrateAgentCockFightPositionTakings));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
