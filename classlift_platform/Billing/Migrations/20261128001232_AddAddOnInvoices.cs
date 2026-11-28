using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddAddOnInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addon_invoices",
                columns: table => new
                {
                    AddOnInvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrganizationID = table.Column<int>(type: "int", nullable: false),
                    OrganizationAddOnSubscriptionID = table.Column<int>(type: "int", nullable: false),
                    AddOnID = table.Column<int>(type: "int", nullable: false),
                    StripeInvoiceId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StripeSubscriptionId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillingPeriodStart = table.Column<DateTime>(type: "datetime", nullable: true),
                    BillingPeriodEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    PaymentFailedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.AddOnInvoiceID);
                    table.ForeignKey(
                        name: "FK_addon_invoices_addons_AddOnID",
                        column: x => x.AddOnID,
                        principalTable: "addons",
                        principalColumn: "AddOnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_addon_invoices_organization_addon_subscriptions_Organization~",
                        column: x => x.OrganizationAddOnSubscriptionID,
                        principalTable: "organization_addon_subscriptions",
                        principalColumn: "OrganizationAddOnSubscriptionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_addon_invoices_organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "organizations",
                        principalColumn: "OrganizationID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_addon_invoices_AddOnID",
                table: "addon_invoices",
                column: "AddOnID");

            migrationBuilder.CreateIndex(
                name: "IX_addon_invoices_OrganizationAddOnSubscriptionID",
                table: "addon_invoices",
                column: "OrganizationAddOnSubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_addon_invoices_OrganizationID",
                table: "addon_invoices",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_addon_invoices_StripeInvoiceId",
                table: "addon_invoices",
                column: "StripeInvoiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addon_invoices");
        }
    }
}
