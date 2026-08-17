using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260817000000_MakePaymentParentOptional")]
public partial class MakePaymentParentOptional : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments");

        migrationBuilder.AlterColumn<int>(
            name: "ParentID",
            table: "payments",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments",
            column: "ParentID",
            principalTable: "parents",
            principalColumn: "ParentID");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments");

        migrationBuilder.Sql(
            "UPDATE payments SET ParentID = (SELECT ParentID FROM parent_child WHERE parent_child.ChildID = payments.ChildID LIMIT 1) WHERE ParentID IS NULL;");

        migrationBuilder.AlterColumn<int>(
            name: "ParentID",
            table: "payments",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments",
            column: "ParentID",
            principalTable: "parents",
            principalColumn: "ParentID",
            onDelete: ReferentialAction.Cascade);
    }
}
