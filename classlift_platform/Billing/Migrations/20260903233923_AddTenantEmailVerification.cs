using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedAt",
                table: "tenantregistry",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationExpiresAt",
                table: "tenantregistry",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationTokenHash",
                table: "tenantregistry",
                type: "char(64)",
                fixedLength: true,
                maxLength: 64,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tenantregistry_EmailVerificationTokenHash",
                table: "tenantregistry",
                column: "EmailVerificationTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenantregistry_EmailVerificationTokenHash",
                table: "tenantregistry");

            migrationBuilder.DropColumn(
                name: "ActivatedAt",
                table: "tenantregistry");

            migrationBuilder.DropColumn(
                name: "EmailVerificationExpiresAt",
                table: "tenantregistry");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenHash",
                table: "tenantregistry");

        }
    }
}
