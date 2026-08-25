using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260815000000_AddChildPostCode")]
public partial class AddChildPostCode : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PostCode",
            table: "children",
            type: "varchar(10)",
            maxLength: 10,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PostCode",
            table: "children");
    }
}
