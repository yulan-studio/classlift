using Billing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations;

/// <summary>
/// Zero-amount invoices do not require customer payment and must not remain
/// Pending or become Overdue.
/// </summary>
[DbContext(typeof(BillingDbContext))]
[Migration("20261128053000_MarkZeroAmountInvoicesPaid")]
public partial class MarkZeroAmountInvoicesPaid : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE `invoices` SET `InvoiceStatus` = 'Paid', `PaidAt` = COALESCE(`PaidAt`, UTC_TIMESTAMP()) WHERE `TotalAmount` = 0 AND `InvoiceStatus` IN ('Pending', 'Overdue');");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // This data correction is intentionally not reversed because the original
        // state cannot be determined reliably after the migration has run.
    }
}
