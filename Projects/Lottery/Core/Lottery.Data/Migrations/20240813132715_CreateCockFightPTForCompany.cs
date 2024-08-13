using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateCockFightPTForCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            var sqlCreateCockFightPTForCompany = Path.Combine(baseScriptDirectory, "CreateCockFightPTForCompany.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlCreateCockFightPTForCompany));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
