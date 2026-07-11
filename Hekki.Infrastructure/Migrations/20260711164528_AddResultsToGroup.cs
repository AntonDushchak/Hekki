using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResultsToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeatGroupEntityId",
                table: "heat_results",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_heat_results_HeatGroupEntityId",
                table: "heat_results",
                column: "HeatGroupEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_heat_results_heat_groups_HeatGroupEntityId",
                table: "heat_results",
                column: "HeatGroupEntityId",
                principalTable: "heat_groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_heat_results_heat_groups_HeatGroupEntityId",
                table: "heat_results");

            migrationBuilder.DropIndex(
                name: "IX_heat_results_HeatGroupEntityId",
                table: "heat_results");

            migrationBuilder.DropColumn(
                name: "HeatGroupEntityId",
                table: "heat_results");
        }
    }
}
