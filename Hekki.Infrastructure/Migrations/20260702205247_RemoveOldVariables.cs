using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "heats");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "heat_results");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "heats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "heat_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
