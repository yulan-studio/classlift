using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260814010000_AddChildContactFields")]
public partial class AddChildContactFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "Phone", table: "children", type: "varchar(50)", maxLength: 50, nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
        migrationBuilder.AddColumn<string>(name: "WeChat", table: "children", type: "varchar(100)", maxLength: 100, nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
        migrationBuilder.AddColumn<string>(name: "WhatsApp", table: "children", type: "varchar(50)", maxLength: 50, nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Phone", table: "children");
        migrationBuilder.DropColumn(name: "WeChat", table: "children");
        migrationBuilder.DropColumn(name: "WhatsApp", table: "children");
    }
}
