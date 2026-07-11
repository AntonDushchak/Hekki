using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHeatIdFromEntryAndResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_heat_entries_heats_HeatId",
                table: "heat_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_heat_results_heats_HeatId",
                table: "heat_results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heat_results",
                table: "heat_results");

            migrationBuilder.DropIndex(
                name: "IX_heat_results_GroupId",
                table: "heat_results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heat_entries",
                table: "heat_entries");

            migrationBuilder.DropIndex(
                name: "IX_heat_entries_GroupId",
                table: "heat_entries");

            migrationBuilder.DropColumn(
                name: "HeatId",
                table: "heat_results");

            migrationBuilder.DropColumn(
                name: "HeatId",
                table: "heat_entries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_heat_results",
                table: "heat_results",
                columns: new[] { "GroupId", "ParticipantId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_heat_entries",
                table: "heat_entries",
                columns: new[] { "GroupId", "ParticipantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_heat_results",
                table: "heat_results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heat_entries",
                table: "heat_entries");

            migrationBuilder.AddColumn<int>(
                name: "HeatId",
                table: "heat_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeatId",
                table: "heat_entries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_heat_results",
                table: "heat_results",
                columns: new[] { "HeatId", "ParticipantId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_heat_entries",
                table: "heat_entries",
                columns: new[] { "HeatId", "ParticipantId" });

            migrationBuilder.CreateIndex(
                name: "IX_heat_results_GroupId",
                table: "heat_results",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_heat_entries_GroupId",
                table: "heat_entries",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_heat_entries_heats_HeatId",
                table: "heat_entries",
                column: "HeatId",
                principalTable: "heats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_heat_results_heats_HeatId",
                table: "heat_results",
                column: "HeatId",
                principalTable: "heats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
