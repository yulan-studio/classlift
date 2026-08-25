using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLift.Diagnostic.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteUrlToDiagnosticLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "diagnostic_leads",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "diagnostic_leads");
        }
    }
}
