using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations;

public partial class SeedActivityManagementAddOn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            INSERT INTO features
                (FeatureKey, FeatureName, Description, IsOrganizationConfigurable, CreatedAt)
            SELECT
                'activity_management',
                'Activity Management',
                'Activity Management feature.',
                1,
                CURRENT_TIMESTAMP
            WHERE NOT EXISTS (
                SELECT 1 FROM features WHERE FeatureKey = 'activity_management'
            );

            INSERT INTO addons
                (AddOnKey, FeatureID, Name, Description, MonthlyPrice, Currency, BillingCycle, IsActive, CreatedAt)
            SELECT
                'activity_management',
                f.FeatureID,
                'Activity Management',
                'Activity Management is billed separately at CAD 30 per month.',
                30.00,
                'CAD',
                'Monthly',
                1,
                CURRENT_TIMESTAMP
            FROM features f
            WHERE f.FeatureKey = 'activity_management'
              AND NOT EXISTS (
                  SELECT 1 FROM addons WHERE AddOnKey = 'activity_management'
              );

            INSERT INTO plan_addon_eligibilities
                (PlanID, AddOnID, IsEligible, CreatedAt)
            SELECT p.PlanID, a.AddOnID, 1, CURRENT_TIMESTAMP
            FROM subscriptionplans p
            CROSS JOIN addons a
            WHERE a.AddOnKey = 'activity_management'
              AND NOT EXISTS (
                  SELECT 1
                  FROM plan_addon_eligibilities e
                  WHERE e.PlanID = p.PlanID AND e.AddOnID = a.AddOnID
              );");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DELETE e
            FROM plan_addon_eligibilities e
            INNER JOIN addons a ON a.AddOnID = e.AddOnID
            WHERE a.AddOnKey = 'activity_management';

            DELETE FROM addons WHERE AddOnKey = 'activity_management';
            DELETE FROM features WHERE FeatureKey = 'activity_management';");
    }
}
