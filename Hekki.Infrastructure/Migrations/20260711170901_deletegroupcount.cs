using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deletegroupcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupCount",
                table: "heats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupCount",
                table: "heats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
