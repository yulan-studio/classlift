using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260817010000_RemovePaymentParent")]
public partial class RemovePaymentParent : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments");

        migrationBuilder.DropIndex(
            name: "IX_payments_ParentID",
            table: "payments");

        migrationBuilder.DropColumn(
            name: "ParentID",
            table: "payments");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ParentID",
            table: "payments",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_payments_ParentID",
            table: "payments",
            column: "ParentID");

        migrationBuilder.AddForeignKey(
            name: "FK_payments_parents_ParentID",
            table: "payments",
            column: "ParentID",
            principalTable: "parents",
            principalColumn: "ParentID");
    }
}
