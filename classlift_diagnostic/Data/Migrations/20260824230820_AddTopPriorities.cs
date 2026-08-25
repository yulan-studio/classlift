using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLift.Diagnostic.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTopPriorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TopPrioritiesJson",
                table: "diagnostic_leads",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopPrioritiesJson",
                table: "diagnostic_leads");
        }
    }
}
