using HnMicro.Core.Helpers;
using Lottery.Data.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseScriptDirectory = MigrationHelper.GetBaseScriptDirectory();
            //  BetKind
            var sqlInitBetKinds = Path.Combine(baseScriptDirectory, "InitBetKinds.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitBetKinds));
            //  Channel
            var sqlInitChannel = Path.Combine(baseScriptDirectory, "InitChannels.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitChannel));
            //  Company User
            var sql = "If NOT EXISTS (Select top 1 1 From Agents Where Username = 'M8XS') " +
                "Begin " +
                "   Declare @agentId bigint; " +
                "   " +
                "   INSERT INTO Agents(" +
                "       ParentId, Username, Password, " +
                "       RoleId, State, Credit, " +
                "       Lock, Permissions, SupermasterId, " +
                "       MasterId, CreatedAt, CreatedBy) " +
                "   Values (" +
                "       0, 'M8XS', '" + "QaWsEd135!#%".Md5() + "', " +
                "       0, 0, 0, " +
                "       0, '[AV],[AU],[AFC],[R],[BL],[VL]', 0, " +
                "       0, GETDATE(), 0); " +
                "   " +
                "   Set @agentId = SCOPE_IDENTITY();" +
                "   " +
                "   INSERT INTO AgentSessions (AgentId, State) VALUES (@agentId, 0); " +
                "End ";
            migrationBuilder.Sql(sql);
            //  Update Odd & PT
            var sqlInitDefaultOdd = Path.Combine(baseScriptDirectory, "CreateOddForCompany.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitDefaultOdd));
            var sqlInitDefaultPositionTaking = Path.Combine(baseScriptDirectory, "CreatePTForCompany.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlInitDefaultPositionTaking));
            ////  Child User - Don't need use below migration.
            //var sqlCreateChildUser = Path.Combine(baseScriptDirectory, "CreateSupermasterMasterAgentPlayer.sql");
            //migrationBuilder.Sql(File.ReadAllText(sqlCreateChildUser).Replace("<ENCRYPTED_PASSWORD>", "QaWsEd135!#%".Md5()));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
