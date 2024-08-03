using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAllMixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlInitBetKinds = Path.Combine(baseScriptDirectory, "MigrationAllMixed.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitBetKinds));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
