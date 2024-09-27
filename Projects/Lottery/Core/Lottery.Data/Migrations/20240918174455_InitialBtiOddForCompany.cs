using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialBtiOddForCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlFile = Path.Combine(baseScriptDirectory, "CreateBtiOddForCompany.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
