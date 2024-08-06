using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lottery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCAPartnerDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CAAgentHandicap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAAgentHandicap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CABetKind",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookieId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLive = table.Column<bool>(type: "bit", nullable: true),
                    OrderInCategory = table.Column<int>(type: "int", nullable: true),
                    Award = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CABetKind", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CABetKind_BookieSettings_BookieId",
                        column: x => x.BookieId,
                        principalTable: "BookieSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAGameType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    GameCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAGameType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CAPlayerMapping",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    BookiePlayerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAccountEnable = table.Column<bool>(type: "bit", nullable: false),
                    IsAlowedToBet = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPlayerMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAPlayerMapping_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAAgentBetSetting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    CABetKindId = table.Column<long>(type: "bigint", nullable: false),
                    DefaultVipHandicapId = table.Column<long>(type: "bigint", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxWin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxLose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAAgentBetSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAAgentBetSetting_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAAgentBetSetting_CAAgentHandicap_DefaultVipHandicapId",
                        column: x => x.DefaultVipHandicapId,
                        principalTable: "CAAgentHandicap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAAgentBetSetting_CABetKind_CABetKindId",
                        column: x => x.CABetKindId,
                        principalTable: "CABetKind",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAAgentPositionTaking",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    CABetKindId = table.Column<long>(type: "bigint", nullable: false),
                    PositionTaking = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAAgentPositionTaking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAAgentPositionTaking_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAAgentPositionTaking_CABetKind_CABetKindId",
                        column: x => x.CABetKindId,
                        principalTable: "CABetKind",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAPlayerBetSetting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerMappingId = table.Column<long>(type: "bigint", nullable: false),
                    CABetKindId = table.Column<long>(type: "bigint", nullable: false),
                    VipHandicapId = table.Column<long>(type: "bigint", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxBet = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxWin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxLose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPlayerBetSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAPlayerBetSetting_CAAgentHandicap_VipHandicapId",
                        column: x => x.VipHandicapId,
                        principalTable: "CAAgentHandicap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAPlayerBetSetting_CABetKind_CABetKindId",
                        column: x => x.CABetKindId,
                        principalTable: "CABetKind",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAPlayerBetSetting_CAPlayerMapping_PlayerMappingId",
                        column: x => x.PlayerMappingId,
                        principalTable: "CAPlayerMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAAgentBetSettingAgentHandicap",
                columns: table => new
                {
                    CAAgentBetSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CAAgentHandicapId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAAgentBetSettingAgentHandicap", x => new { x.CAAgentBetSettingId, x.CAAgentHandicapId });
                    table.ForeignKey(
                        name: "FK_CAAgentBetSettingAgentHandicap_CAAgentBetSetting_CAAgentBetSettingId",
                        column: x => x.CAAgentBetSettingId,
                        principalTable: "CAAgentBetSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CAAgentBetSettingAgentHandicap_CAAgentHandicap_CAAgentHandicapId",
                        column: x => x.CAAgentHandicapId,
                        principalTable: "CAAgentHandicap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAPlayerBetSettingAgentHandicap",
                columns: table => new
                {
                    CAPlayerBetSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CAAgentHandicapId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPlayerBetSettingAgentHandicap", x => new { x.CAPlayerBetSettingId, x.CAAgentHandicapId });
                    table.ForeignKey(
                        name: "FK_CAPlayerBetSettingAgentHandicap_CAAgentHandicap_CAAgentHandicapId",
                        column: x => x.CAAgentHandicapId,
                        principalTable: "CAAgentHandicap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAPlayerBetSettingAgentHandicap_CAPlayerBetSetting_CAPlayerBetSettingId",
                        column: x => x.CAPlayerBetSettingId,
                        principalTable: "CAPlayerBetSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentBetSetting_AgentId",
                table: "CAAgentBetSetting",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentBetSetting_CABetKindId",
                table: "CAAgentBetSetting",
                column: "CABetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentBetSetting_DefaultVipHandicapId",
                table: "CAAgentBetSetting",
                column: "DefaultVipHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentBetSettingAgentHandicap_CAAgentHandicapId",
                table: "CAAgentBetSettingAgentHandicap",
                column: "CAAgentHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentHandicap_Name",
                table: "CAAgentHandicap",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentPositionTaking_AgentId",
                table: "CAAgentPositionTaking",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CAAgentPositionTaking_CABetKindId",
                table: "CAAgentPositionTaking",
                column: "CABetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CABetKind_BookieId",
                table: "CABetKind",
                column: "BookieId");

            migrationBuilder.CreateIndex(
                name: "IX_CABetKind_Code",
                table: "CABetKind",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerBetSetting_CABetKindId",
                table: "CAPlayerBetSetting",
                column: "CABetKindId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerBetSetting_PlayerMappingId",
                table: "CAPlayerBetSetting",
                column: "PlayerMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerBetSetting_VipHandicapId",
                table: "CAPlayerBetSetting",
                column: "VipHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerBetSettingAgentHandicap_CAAgentHandicapId",
                table: "CAPlayerBetSettingAgentHandicap",
                column: "CAAgentHandicapId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerMapping_BookiePlayerId",
                table: "CAPlayerMapping",
                column: "BookiePlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAPlayerMapping_PlayerId",
                table: "CAPlayerMapping",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CAAgentBetSettingAgentHandicap");

            migrationBuilder.DropTable(
                name: "CAAgentPositionTaking");

            migrationBuilder.DropTable(
                name: "CAGameType");

            migrationBuilder.DropTable(
                name: "CAPlayerBetSettingAgentHandicap");

            migrationBuilder.DropTable(
                name: "CAAgentBetSetting");

            migrationBuilder.DropTable(
                name: "CAPlayerBetSetting");

            migrationBuilder.DropTable(
                name: "CAAgentHandicap");

            migrationBuilder.DropTable(
                name: "CABetKind");

            migrationBuilder.DropTable(
                name: "CAPlayerMapping");
        }
    }
}
