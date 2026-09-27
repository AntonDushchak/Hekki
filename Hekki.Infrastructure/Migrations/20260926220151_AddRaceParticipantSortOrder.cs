using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceParticipantSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "race_participants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE race_participants p
                SET "SortOrder" = s.rn
                FROM (
                    SELECT "Id", ROW_NUMBER() OVER (PARTITION BY "RaceId" ORDER BY "LastName", "FirstName") - 1 AS rn
                    FROM race_participants
                ) s
                WHERE p."Id" = s."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_race_participants_RaceId_SortOrder",
                table: "race_participants",
                columns: new[] { "RaceId", "SortOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_race_participants_RaceId_SortOrder",
                table: "race_participants");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "race_participants");
        }
    }
}
