using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Billing.Data;

#nullable disable

namespace Billing.Migrations;

[DbContext(typeof(BillingDbContext))]
[Migration("20260929000000_UpdatePlanPrices")]
public partial class UpdatePlanPrices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE subscriptionplans SET PricePerCoach = 20.00 WHERE PlanName = 'Starter';");
        migrationBuilder.Sql("UPDATE subscriptionplans SET PricePerCoach = 30.00 WHERE PlanName = 'Grow';");
        migrationBuilder.Sql("UPDATE subscriptionplans SET PricePerCoach = 50.00 WHERE PlanName = 'Pro';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
