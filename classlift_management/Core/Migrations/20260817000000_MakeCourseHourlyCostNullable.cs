using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260817000000_MakeCourseHourlyCostNullable")]
public partial class MakeCourseHourlyCostNullable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "HourlyCost",
            table: "courses",
            type: "decimal(10,2)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(10,2)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE [courses] SET [HourlyCost] = 0 WHERE [HourlyCost] IS NULL");

        migrationBuilder.AlterColumn<decimal>(
            name: "HourlyCost",
            table: "courses",
            type: "decimal(10,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(10,2)",
            oldNullable: true);
    }
}
