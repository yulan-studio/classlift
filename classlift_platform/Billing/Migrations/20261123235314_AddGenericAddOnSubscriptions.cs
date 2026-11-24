using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddGenericAddOnSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addons",
                columns: table => new
                {
                    AddOnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AddOnKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FeatureID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "CAD", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillingCycle = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Monthly", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.AddOnID);
                    table.ForeignKey(
                        name: "FK_AddOns_Features",
                        column: x => x.FeatureID,
                        principalTable: "features",
                        principalColumn: "FeatureID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "organization_addon_subscriptions",
                columns: table => new
                {
                    OrganizationAddOnSubscriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrganizationID = table.Column<int>(type: "int", nullable: false),
                    AddOnID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "CAD", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillingCycle = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Monthly", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutoRenew = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.OrganizationAddOnSubscriptionID);
                    table.ForeignKey(
                        name: "FK_OrgAddOnSub_AddOns",
                        column: x => x.AddOnID,
                        principalTable: "addons",
                        principalColumn: "AddOnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrgAddOnSub_Organizations",
                        column: x => x.OrganizationID,
                        principalTable: "organizations",
                        principalColumn: "OrganizationID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "plan_addon_eligibilities",
                columns: table => new
                {
                    PlanAddOnEligibilityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    AddOnID = table.Column<int>(type: "int", nullable: false),
                    IsEligible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.PlanAddOnEligibilityID);
                    table.ForeignKey(
                        name: "FK_PlanAddOnEligibility_AddOns",
                        column: x => x.AddOnID,
                        principalTable: "addons",
                        principalColumn: "AddOnID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanAddOnEligibility_Plans",
                        column: x => x.PlanID,
                        principalTable: "subscriptionplans",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IDX_AddOns_FeatureID",
                table: "addons",
                column: "FeatureID");

            migrationBuilder.CreateIndex(
                name: "UX_AddOns_AddOnKey",
                table: "addons",
                column: "AddOnKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_OrgAddOnSub_AddOnID",
                table: "organization_addon_subscriptions",
                column: "AddOnID");

            migrationBuilder.CreateIndex(
                name: "IDX_OrgAddOnSub_OrganizationID",
                table: "organization_addon_subscriptions",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IDX_OrgAddOnSub_Status",
                table: "organization_addon_subscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_PlanAddOnEligibility_AddOnID",
                table: "plan_addon_eligibilities",
                column: "AddOnID");

            migrationBuilder.CreateIndex(
                name: "UX_PlanAddOnEligibility",
                table: "plan_addon_eligibilities",
                columns: new[] { "PlanID", "AddOnID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_addon_subscriptions");

            migrationBuilder.DropTable(
                name: "plan_addon_eligibilities");

            migrationBuilder.DropTable(
                name: "addons");

        }
    }
}
