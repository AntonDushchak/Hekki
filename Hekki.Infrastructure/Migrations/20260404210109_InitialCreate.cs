using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pilots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pilots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "regulations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Json = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regulations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "races",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RegulationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_races", x => x.Id);
                    table.ForeignKey(
                        name: "FK_races_regulations_RegulationId",
                        column: x => x.RegulationId,
                        principalTable: "regulations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "heats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RaceId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RoleLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegulationId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_heats_races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_heats_regulations_RegulationId",
                        column: x => x.RegulationId,
                        principalTable: "regulations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "race_participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RaceId = table.Column<int>(type: "integer", nullable: false),
                    PilotId = table.Column<int>(type: "integer", nullable: false),
                    Team = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_race_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_race_participants_pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_race_participants_races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "heat_entries",
                columns: table => new
                {
                    HeatId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    SeedOrder = table.Column<int>(type: "integer", nullable: false),
                    GridPosition = table.Column<int>(type: "integer", nullable: true),
                    KartNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heat_entries", x => new { x.HeatId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_heat_entries_heats_HeatId",
                        column: x => x.HeatId,
                        principalTable: "heats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_heat_entries_race_participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "race_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "heat_results",
                columns: table => new
                {
                    HeatId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    FinishPosition = table.Column<int>(type: "integer", nullable: true),
                    TotalTimeMs = table.Column<long>(type: "bigint", nullable: true),
                    BestLapMs = table.Column<long>(type: "bigint", nullable: true),
                    Laps = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heat_results", x => new { x.HeatId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_heat_results_heats_HeatId",
                        column: x => x.HeatId,
                        principalTable: "heats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_heat_results_race_participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "race_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_heat_entries_ParticipantId",
                table: "heat_entries",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_heat_results_ParticipantId",
                table: "heat_results",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_heats_RaceId",
                table: "heats",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_heats_RegulationId",
                table: "heats",
                column: "RegulationId");

            migrationBuilder.CreateIndex(
                name: "IX_race_participants_PilotId",
                table: "race_participants",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_race_participants_RaceId_PilotId",
                table: "race_participants",
                columns: new[] { "RaceId", "PilotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_races_RegulationId",
                table: "races",
                column: "RegulationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "heat_entries");

            migrationBuilder.DropTable(
                name: "heat_results");

            migrationBuilder.DropTable(
                name: "heats");

            migrationBuilder.DropTable(
                name: "race_participants");

            migrationBuilder.DropTable(
                name: "pilots");

            migrationBuilder.DropTable(
                name: "races");

            migrationBuilder.DropTable(
                name: "regulations");
        }
    }
}
