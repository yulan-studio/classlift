using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations;

public partial class BackfillActivityManagementPlanEligibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            INSERT INTO plan_addon_eligibilities
                (PlanID, AddOnID, IsEligible, CreatedAt)
            SELECT
                p.PlanID,
                a.AddOnID,
                1,
                CURRENT_TIMESTAMP
            FROM subscriptionplans p
            CROSS JOIN addons a
            WHERE a.AddOnKey = 'activity_management'
              AND NOT EXISTS (
                  SELECT 1
                  FROM plan_addon_eligibilities e
                  WHERE e.PlanID = p.PlanID
                    AND e.AddOnID = a.AddOnID
              );");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Intentionally left empty. This migration backfills missing eligibility
        // rows and cannot safely distinguish them from pre-existing rows.
    }
}
