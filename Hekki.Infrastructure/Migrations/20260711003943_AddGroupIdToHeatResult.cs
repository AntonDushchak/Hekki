using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToHeatResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "heat_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_heat_results_GroupId",
                table: "heat_results",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_heat_results_heat_groups_GroupId",
                table: "heat_results",
                column: "GroupId",
                principalTable: "heat_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_heat_results_heat_groups_GroupId",
                table: "heat_results");

            migrationBuilder.DropIndex(
                name: "IX_heat_results_GroupId",
                table: "heat_results");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "heat_results");
        }
    }
}
