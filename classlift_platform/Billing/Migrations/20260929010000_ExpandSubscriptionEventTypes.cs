using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Billing.Data;

#nullable disable

namespace Billing.Migrations;

[DbContext(typeof(BillingDbContext))]
[Migration("20260929010000_ExpandSubscriptionEventTypes")]
public partial class ExpandSubscriptionEventTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "ALTER TABLE subscription_events MODIFY COLUMN EventType enum('Created','Activated','PlanChanged','Cancelled','Expired','Suspended','Reactivated','TrialStarted','TrialEnded','PaymentReceived') NOT NULL;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "ALTER TABLE subscription_events MODIFY COLUMN EventType enum('Created','Activated','PlanChanged','Cancelled','Expired','Suspended','Reactivated') NOT NULL;");
    }
}
