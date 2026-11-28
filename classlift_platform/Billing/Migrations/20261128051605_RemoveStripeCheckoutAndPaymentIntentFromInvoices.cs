using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStripeCheckoutAndPaymentIntentFromInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invoices_StripeCheckoutSessionId",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_StripePaymentIntentId",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "StripeCheckoutSessionId",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "invoices",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "invoices",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_StripeCheckoutSessionId",
                table: "invoices",
                column: "StripeCheckoutSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoices_StripePaymentIntentId",
                table: "invoices",
                column: "StripePaymentIntentId",
                unique: true);
        }
    }
}
