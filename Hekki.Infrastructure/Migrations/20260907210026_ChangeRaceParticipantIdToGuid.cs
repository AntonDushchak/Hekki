using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRaceParticipantIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE race_participants ADD COLUMN \"__new_id\" uuid;");
            migrationBuilder.Sql("UPDATE race_participants SET \"__new_id\" = gen_random_uuid();");

            migrationBuilder.Sql("ALTER TABLE heat_entries ADD COLUMN \"__new_participant_id\" uuid;");
            migrationBuilder.Sql("ALTER TABLE heat_results ADD COLUMN \"__new_participant_id\" uuid;");

            migrationBuilder.Sql("""
                UPDATE heat_entries AS he
                SET "__new_participant_id" = rp."__new_id"
                FROM race_participants AS rp
                WHERE he."ParticipantId" = rp."Id";
                """);
            migrationBuilder.Sql("""
                UPDATE heat_results AS hr
                SET "__new_participant_id" = rp."__new_id"
                FROM race_participants AS rp
                WHERE hr."ParticipantId" = rp."Id";
                """);

            migrationBuilder.Sql("ALTER TABLE heat_entries DROP CONSTRAINT \"FK_heat_entries_race_participants_ParticipantId\";");
            migrationBuilder.Sql("ALTER TABLE heat_results DROP CONSTRAINT \"FK_heat_results_race_participants_ParticipantId\";");
            migrationBuilder.Sql("DROP INDEX \"IX_heat_entries_ParticipantId\";");
            migrationBuilder.Sql("DROP INDEX \"IX_heat_results_ParticipantId\";");
            migrationBuilder.Sql("ALTER TABLE heat_entries DROP CONSTRAINT \"PK_heat_entries\";");
            migrationBuilder.Sql("ALTER TABLE heat_results DROP CONSTRAINT \"PK_heat_results\";");
            migrationBuilder.Sql("ALTER TABLE race_participants DROP CONSTRAINT \"PK_race_participants\";");

            migrationBuilder.Sql("ALTER TABLE heat_entries DROP COLUMN \"ParticipantId\";");
            migrationBuilder.Sql("ALTER TABLE heat_results DROP COLUMN \"ParticipantId\";");
            migrationBuilder.Sql("ALTER TABLE race_participants DROP COLUMN \"Id\";");

            migrationBuilder.Sql("ALTER TABLE race_participants RENAME COLUMN \"__new_id\" TO \"Id\";");
            migrationBuilder.Sql("ALTER TABLE heat_entries RENAME COLUMN \"__new_participant_id\" TO \"ParticipantId\";");
            migrationBuilder.Sql("ALTER TABLE heat_results RENAME COLUMN \"__new_participant_id\" TO \"ParticipantId\";");

            migrationBuilder.Sql("ALTER TABLE race_participants ALTER COLUMN \"Id\" SET NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE heat_entries ALTER COLUMN \"ParticipantId\" SET NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE heat_results ALTER COLUMN \"ParticipantId\" SET NOT NULL;");

            migrationBuilder.Sql("ALTER TABLE race_participants ADD CONSTRAINT \"PK_race_participants\" PRIMARY KEY (\"Id\");");
            migrationBuilder.Sql("ALTER TABLE heat_entries ADD CONSTRAINT \"PK_heat_entries\" PRIMARY KEY (\"GroupId\", \"ParticipantId\");");
            migrationBuilder.Sql("ALTER TABLE heat_results ADD CONSTRAINT \"PK_heat_results\" PRIMARY KEY (\"GroupId\", \"ParticipantId\");");
            migrationBuilder.Sql("CREATE INDEX \"IX_heat_entries_ParticipantId\" ON heat_entries (\"ParticipantId\");");
            migrationBuilder.Sql("CREATE INDEX \"IX_heat_results_ParticipantId\" ON heat_results (\"ParticipantId\");");
            migrationBuilder.Sql("""
                ALTER TABLE heat_entries
                ADD CONSTRAINT "FK_heat_entries_race_participants_ParticipantId"
                FOREIGN KEY ("ParticipantId") REFERENCES race_participants ("Id") ON DELETE CASCADE;
                """);
            migrationBuilder.Sql("""
                ALTER TABLE heat_results
                ADD CONSTRAINT "FK_heat_results_race_participants_ParticipantId"
                FOREIGN KEY ("ParticipantId") REFERENCES race_participants ("Id") ON DELETE CASCADE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new InvalidOperationException(
                "Cannot safely revert race participant UUIDs to integer IDs because the original integer mapping is not preserved.");
        }
    }
}
