using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewVariable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "League",
                table: "race_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "League",
                table: "pilots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "pilots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeatNumber",
                table: "heats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "League",
                table: "race_participants");

            migrationBuilder.DropColumn(
                name: "League",
                table: "pilots");

            migrationBuilder.DropColumn(
                name: "Team",
                table: "pilots");

            migrationBuilder.DropColumn(
                name: "HeatNumber",
                table: "heats");
        }
    }
}
