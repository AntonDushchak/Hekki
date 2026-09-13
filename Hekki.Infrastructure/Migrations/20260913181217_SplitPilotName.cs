using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hekki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitPilotName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "pilots",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "pilots",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE pilots
                SET "FirstName" = split_part(btrim("Name"), ' ', 1),
                    "LastName" = btrim(substr(btrim("Name"), length(split_part(btrim("Name"), ' ', 1)) + 1))
                WHERE "Name" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "Name",
                table: "pilots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "pilots",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE pilots
                SET "Name" = btrim(concat("FirstName", ' ', "LastName"));
                """);

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "pilots");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "pilots");
        }
    }
}
