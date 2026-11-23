using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

public partial class AddActivityEnrollmentToChildBalance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ActivityEnrollmentID",
            table: "child_balance",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_child_balance_ActivityEnrollmentID",
            table: "child_balance",
            column: "ActivityEnrollmentID");

        migrationBuilder.AddForeignKey(
            name: "FK_child_balance_activity_enrollments_ActivityEnrollmentID",
            table: "child_balance",
            column: "ActivityEnrollmentID",
            principalTable: "activity_enrollments",
            principalColumn: "EnrollmentID");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_child_balance_activity_enrollments_ActivityEnrollmentID",
            table: "child_balance");

        migrationBuilder.DropIndex(
            name: "IX_child_balance_ActivityEnrollmentID",
            table: "child_balance");

        migrationBuilder.DropColumn(
            name: "ActivityEnrollmentID",
            table: "child_balance");
    }
}
