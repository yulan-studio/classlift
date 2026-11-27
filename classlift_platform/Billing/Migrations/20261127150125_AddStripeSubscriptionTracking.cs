using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeSubscriptionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancelAtPeriodEnd",
                table: "organization_addon_subscriptions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "organization_addon_subscriptions",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UX_OrgAddOnSub_StripeSubscriptionId",
                table: "organization_addon_subscriptions",
                column: "StripeSubscriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_OrgAddOnSub_StripeSubscriptionId",
                table: "organization_addon_subscriptions");

            migrationBuilder.DropColumn(
                name: "CancelAtPeriodEnd",
                table: "organization_addon_subscriptions");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "organization_addon_subscriptions");
        }
    }
}
