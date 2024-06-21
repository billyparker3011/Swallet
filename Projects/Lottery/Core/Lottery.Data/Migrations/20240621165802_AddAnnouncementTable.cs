using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentAnnouncements_Announcements_AnnouncementId",
                table: "AgentAnnouncements");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "AgentAnnouncements",
                newName: "AgentId");

            migrationBuilder.RenameColumn(
                name: "AgentAnnouncementType",
                table: "AgentAnnouncements",
                newName: "AnnouncementType");

            migrationBuilder.RenameIndex(
                name: "IX_AgentAnnouncements_TargetId",
                table: "AgentAnnouncements",
                newName: "IX_AgentAnnouncements_AgentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnnouncementType",
                table: "AgentAnnouncements",
                newName: "AgentAnnouncementType");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "AgentAnnouncements",
                newName: "TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_AgentAnnouncements_AgentId",
                table: "AgentAnnouncements",
                newName: "IX_AgentAnnouncements_TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentAnnouncements_Announcements_AnnouncementId",
                table: "AgentAnnouncements",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id");
        }
    }
}
