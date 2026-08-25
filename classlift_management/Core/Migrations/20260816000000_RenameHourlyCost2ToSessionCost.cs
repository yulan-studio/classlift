using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260816000000_RenameHourlyCost2ToSessionCost")]
public partial class RenameHourlyCost2ToSessionCost : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "HourlyCost2",
            table: "courses",
            newName: "SessionCost");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "SessionCost",
            table: "courses",
            newName: "HourlyCost2");
    }
}
