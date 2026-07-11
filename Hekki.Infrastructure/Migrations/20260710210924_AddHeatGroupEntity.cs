using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHeatGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupNumber",
                table: "heat_entries",
                newName: "GroupId");

            migrationBuilder.AddColumn<int>(
                name: "Penalty",
                table: "heat_results",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "heat_results",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "heat_groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeatId = table.Column<int>(type: "integer", nullable: false),
                    GroupIndex = table.Column<int>(type: "integer", nullable: false),
                    GroupNumber = table.Column<int>(type: "integer", nullable: false),
                    GroupCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_heat_groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_heat_groups_heats_HeatId",
                        column: x => x.HeatId,
                        principalTable: "heats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_heat_entries_GroupId",
                table: "heat_entries",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_heat_groups_HeatId",
                table: "heat_groups",
                column: "HeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_heat_entries_heat_groups_GroupId",
                table: "heat_entries",
                column: "GroupId",
                principalTable: "heat_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_heat_entries_heat_groups_GroupId",
                table: "heat_entries");

            migrationBuilder.DropTable(
                name: "heat_groups");

            migrationBuilder.DropIndex(
                name: "IX_heat_entries_GroupId",
                table: "heat_entries");

            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "heat_results");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "heat_results");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "heat_entries",
                newName: "GroupNumber");
        }
    }
}
