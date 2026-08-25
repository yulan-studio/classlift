using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLift.Diagnostic.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyDiagnosticQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNeeds",
                table: "diagnostic_leads",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImprovementAreasJson",
                table: "diagnostic_leads",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNeeds",
                table: "diagnostic_leads");

            migrationBuilder.DropColumn(
                name: "ImprovementAreasJson",
                table: "diagnostic_leads");
        }
    }
}
