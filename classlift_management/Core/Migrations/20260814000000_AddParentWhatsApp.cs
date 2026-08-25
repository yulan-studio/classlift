using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260814000000_AddParentWhatsApp")]
public partial class AddParentWhatsApp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "WhatsApp",
            table: "parents",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WhatsApp",
            table: "parents");
    }
}
