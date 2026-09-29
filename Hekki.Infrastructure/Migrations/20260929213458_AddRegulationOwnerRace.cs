using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegulationOwnerRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerRaceId",
                table: "regulations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerRaceId",
                table: "regulations");
        }
    }
}
