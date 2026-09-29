using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations;

public partial class RemoveMinimumMonthlyPrice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MinimumMonthlyPrice",
            table: "organization_subscriptions");

        migrationBuilder.DropColumn(
            name: "MinimumMonthlyPrice",
            table: "subscriptionplans");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "MinimumMonthlyPrice",
            table: "organization_subscriptions",
            type: "decimal(10,2)",
            precision: 10,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "MinimumMonthlyPrice",
            table: "subscriptionplans",
            type: "decimal(10,2)",
            precision: 10,
            scale: 2,
            nullable: false,
            defaultValue: 0m);
    }
}
