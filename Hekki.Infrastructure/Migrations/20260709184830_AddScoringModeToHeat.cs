using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScoringModeToHeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScoringMode",
                table: "heats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoringMode",
                table: "heats");
        }
    }
}
