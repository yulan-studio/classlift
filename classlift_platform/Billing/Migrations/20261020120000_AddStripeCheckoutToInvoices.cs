using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Billing.Migrations;

[DbContext(typeof(Billing.Data.BillingDbContext))]
[Migration("20261020120000_AddStripeCheckoutToInvoices")]
public partial class AddStripeCheckoutToInvoices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("StripeCheckoutSessionId", "invoices", maxLength: 255, nullable: true);
        migrationBuilder.AddColumn<string>("StripePaymentIntentId", "invoices", maxLength: 255, nullable: true);
        migrationBuilder.CreateIndex("IX_invoices_StripeCheckoutSessionId", "invoices", "StripeCheckoutSessionId", unique: true);
        migrationBuilder.CreateIndex("IX_invoices_StripePaymentIntentId", "invoices", "StripePaymentIntentId", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_invoices_StripeCheckoutSessionId", "invoices");
        migrationBuilder.DropIndex("IX_invoices_StripePaymentIntentId", "invoices");
        migrationBuilder.DropColumn("StripeCheckoutSessionId", "invoices");
        migrationBuilder.DropColumn("StripePaymentIntentId", "invoices");
    }
}
