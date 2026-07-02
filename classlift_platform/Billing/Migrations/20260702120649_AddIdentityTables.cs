using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "billing_runs",
            //    columns: table => new
            //    {
            //        BillingRunID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        RunType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        StartedAt = table.Column<DateTime>(type: "datetime", nullable: false),
            //        FinishedAt = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        DurationMilliseconds = table.Column<int>(type: "int", nullable: true),
            //        ErrorMessage = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        TrialActivated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
            //        InvoicesGenerated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
            //        InvoicesMarkedOverdue = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.BillingRunID);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "features",
            //    columns: table => new
            //    {
            //        FeatureID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        FeatureKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        FeatureName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.FeatureID);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "subscriptionplans",
            //    columns: table => new
            //    {
            //        PlanID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        PlanName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PricePerCoach = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        MinimumMonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.PlanID);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "organizations",
            //    columns: table => new
            //    {
            //        OrganizationID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        OrganizationName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ContactName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ContactEmail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ContactPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CurrentPlanID = table.Column<int>(type: "int", nullable: true),
            //        IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.OrganizationID);
            //        table.ForeignKey(
            //            name: "FK_Organizations_Plans",
            //            column: x => x.CurrentPlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID");
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "planfeatures",
            //    columns: table => new
            //    {
            //        PlanFeatureID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        PlanID = table.Column<int>(type: "int", nullable: false),
            //        FeatureID = table.Column<int>(type: "int", nullable: false),
            //        IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.PlanFeatureID);
            //        table.ForeignKey(
            //            name: "FK_PlanFeatures_Features",
            //            column: x => x.FeatureID,
            //            principalTable: "features",
            //            principalColumn: "FeatureID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PlanFeatures_Plans",
            //            column: x => x.PlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID",
            //            onDelete: ReferentialAction.Cascade);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "promotions",
            //    columns: table => new
            //    {
            //        PromotionID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        PromotionName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PlanID = table.Column<int>(type: "int", nullable: false),
            //        DiscountType = table.Column<string>(type: "enum('Percentage','FixedAmount','PriceOverride')", nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        DiscountValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        DurationMonths = table.Column<int>(type: "int", nullable: false),
            //        StartDate = table.Column<DateOnly>(type: "date", nullable: true),
            //        EndDate = table.Column<DateOnly>(type: "date", nullable: true),
            //        IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.PromotionID);
            //        table.ForeignKey(
            //            name: "FK_Promotions_Plans",
            //            column: x => x.PlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID",
            //            onDelete: ReferentialAction.Cascade);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "organization_subscriptions",
            //    columns: table => new
            //    {
            //        OrganizationSubscriptionID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        OrganizationID = table.Column<int>(type: "int", nullable: false),
            //        PlanID = table.Column<int>(type: "int", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Status = table.Column<string>(type: "enum('Active','Cancelled','Expired')", nullable: false, defaultValueSql: "'Active'", collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        IsTrial = table.Column<sbyte>(type: "tinyint", nullable: false),
            //        TrialStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        TrialEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        ActivatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        CancelledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        LastBilledDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        AutoRenew = table.Column<sbyte>(type: "tinyint", nullable: false),
            //        MonthlyPricePerCoach = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        MinimumMonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
            //        organization_subscriptionscol = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.OrganizationSubscriptionID);
            //        table.ForeignKey(
            //            name: "FK_OrgSub_Organizations",
            //            column: x => x.OrganizationID,
            //            principalTable: "organizations",
            //            principalColumn: "OrganizationID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_OrgSub_Plans",
            //            column: x => x.PlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID");
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "tenantregistry",
            //    columns: table => new
            //    {
            //        TenantRegistryID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        OrganizationID = table.Column<int>(type: "int", nullable: false),
            //        DatabaseName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Subdomain = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CustomDomain = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.TenantRegistryID);
            //        table.ForeignKey(
            //            name: "FK_TenantRegistry_Organizations",
            //            column: x => x.OrganizationID,
            //            principalTable: "organizations",
            //            principalColumn: "OrganizationID",
            //            onDelete: ReferentialAction.Cascade);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "invoices",
            //    columns: table => new
            //    {
            //        InvoiceID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        OrganizationID = table.Column<int>(type: "int", nullable: false),
            //        OrganizationSubscriptionID = table.Column<int>(type: "int", nullable: false),
            //        PlanID = table.Column<int>(type: "int", nullable: false),
            //        PromotionID = table.Column<int>(type: "int", nullable: true),
            //        BillingPeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
            //        BillingPeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
            //        DueDate = table.Column<DateOnly>(type: "date", nullable: false),
            //        CoachCount = table.Column<int>(type: "int", nullable: false),
            //        PricePerCoach = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        Subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        InvoiceStatus = table.Column<string>(type: "enum('Pending','Paid','Cancelled','Overdue')", nullable: false, defaultValueSql: "'Pending'", collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        GeneratedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        PaidAt = table.Column<DateTime>(type: "datetime", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.InvoiceID);
            //        table.ForeignKey(
            //            name: "FK_Invoices_OrganizationSubscriptions",
            //            column: x => x.OrganizationSubscriptionID,
            //            principalTable: "organization_subscriptions",
            //            principalColumn: "OrganizationSubscriptionID");
            //        table.ForeignKey(
            //            name: "FK_Invoices_Organizations",
            //            column: x => x.OrganizationID,
            //            principalTable: "organizations",
            //            principalColumn: "OrganizationID");
            //        table.ForeignKey(
            //            name: "FK_Invoices_Plans",
            //            column: x => x.PlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID");
            //        table.ForeignKey(
            //            name: "FK_Invoices_Promotions",
            //            column: x => x.PromotionID,
            //            principalTable: "promotions",
            //            principalColumn: "PromotionID");
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "subscription_events",
            //    columns: table => new
            //    {
            //        SubscriptionEventID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        OrganizationID = table.Column<int>(type: "int", nullable: false),
            //        OrganizationSubscriptionID = table.Column<int>(type: "int", nullable: true),
            //        EventType = table.Column<string>(type: "enum('Created','Activated','PlanChanged','Cancelled','Expired','Suspended','Reactivated')", nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        OldPlanID = table.Column<int>(type: "int", nullable: true),
            //        NewPlanID = table.Column<int>(type: "int", nullable: true),
            //        OldStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        NewStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        EffectiveAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Reason = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.SubscriptionEventID);
            //        table.ForeignKey(
            //            name: "FK_SubEvents_NewPlan",
            //            column: x => x.NewPlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID");
            //        table.ForeignKey(
            //            name: "FK_SubEvents_OldPlan",
            //            column: x => x.OldPlanID,
            //            principalTable: "subscriptionplans",
            //            principalColumn: "PlanID");
            //        table.ForeignKey(
            //            name: "FK_SubEvents_Organizations",
            //            column: x => x.OrganizationID,
            //            principalTable: "organizations",
            //            principalColumn: "OrganizationID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_SubEvents_Subscriptions",
            //            column: x => x.OrganizationSubscriptionID,
            //            principalTable: "organization_subscriptions",
            //            principalColumn: "OrganizationSubscriptionID",
            //            onDelete: ReferentialAction.SetNull);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            //migrationBuilder.CreateTable(
            //    name: "payments",
            //    columns: table => new
            //    {
            //        PaymentID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        InvoiceID = table.Column<int>(type: "int", nullable: false),
            //        PaymentProvider = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ProviderTransactionID = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //        Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValueSql: "'CAD'", collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PaymentStatus = table.Column<string>(type: "enum('Pending','Succeeded','Failed','Refunded')", nullable: false, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        Notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PRIMARY", x => x.PaymentID);
            //        table.ForeignKey(
            //            name: "FK_Payments_Invoices",
            //            column: x => x.InvoiceID,
            //            principalTable: "invoices",
            //            principalColumn: "InvoiceID",
            //            onDelete: ReferentialAction.Cascade);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4")
            //    .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "FeatureKey",
            //    table: "features",
            //    column: "FeatureKey",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "FK_Invoices_Plans",
            //    table: "invoices",
            //    column: "PlanID");

            //migrationBuilder.CreateIndex(
            //    name: "FK_Invoices_Promotions",
            //    table: "invoices",
            //    column: "PromotionID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_Invoices_OrganizationID",
            //    table: "invoices",
            //    column: "OrganizationID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_Invoices_OrganizationSubscriptionID",
            //    table: "invoices",
            //    column: "OrganizationSubscriptionID");

            //migrationBuilder.CreateIndex(
            //    name: "UX_Invoice_Subscription_BillingPeriod",
            //    table: "invoices",
            //    columns: new[] { "OrganizationSubscriptionID", "BillingPeriodStart", "BillingPeriodEnd" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IDX_OrgSub_OrganizationID",
            //    table: "organization_subscriptions",
            //    column: "OrganizationID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_OrgSub_PlanID",
            //    table: "organization_subscriptions",
            //    column: "PlanID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_OrgSub_Status",
            //    table: "organization_subscriptions",
            //    column: "Status");

            //migrationBuilder.CreateIndex(
            //    name: "FK_Organizations_Plans",
            //    table: "organizations",
            //    column: "CurrentPlanID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_Organizations_IsActive",
            //    table: "organizations",
            //    column: "IsActive");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_Payments_InvoiceID",
            //    table: "payments",
            //    column: "InvoiceID");

            //migrationBuilder.CreateIndex(
            //    name: "FK_PlanFeatures_Features",
            //    table: "planfeatures",
            //    column: "FeatureID");

            //migrationBuilder.CreateIndex(
            //    name: "UK_Plan_Feature",
            //    table: "planfeatures",
            //    columns: new[] { "PlanID", "FeatureID" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "FK_Promotions_Plans",
            //    table: "promotions",
            //    column: "PlanID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_Promotions_IsActive",
            //    table: "promotions",
            //    column: "IsActive");

            //migrationBuilder.CreateIndex(
            //    name: "FK_SubEvents_NewPlan",
            //    table: "subscription_events",
            //    column: "NewPlanID");

            //migrationBuilder.CreateIndex(
            //    name: "FK_SubEvents_OldPlan",
            //    table: "subscription_events",
            //    column: "OldPlanID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_SubEvents_EffectiveAt",
            //    table: "subscription_events",
            //    column: "EffectiveAt");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_SubEvents_EventType",
            //    table: "subscription_events",
            //    column: "EventType");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_SubEvents_OrganizationID",
            //    table: "subscription_events",
            //    column: "OrganizationID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_SubEvents_SubscriptionID",
            //    table: "subscription_events",
            //    column: "OrganizationSubscriptionID");

            //migrationBuilder.CreateIndex(
            //    name: "IDX_SubscriptionPlans_IsActive",
            //    table: "subscriptionplans",
            //    column: "IsActive");

            //migrationBuilder.CreateIndex(
            //    name: "PlanName",
            //    table: "subscriptionplans",
            //    column: "PlanName",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IDX_TenantRegistry_OrganizationID",
            //    table: "tenantregistry",
            //    column: "OrganizationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            //migrationBuilder.DropTable(
            //    name: "billing_runs");

            //migrationBuilder.DropTable(
            //    name: "payments");

            //migrationBuilder.DropTable(
            //    name: "planfeatures");

            //migrationBuilder.DropTable(
            //    name: "subscription_events");

            //migrationBuilder.DropTable(
            //    name: "tenantregistry");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            //migrationBuilder.DropTable(
            //    name: "invoices");

            //migrationBuilder.DropTable(
            //    name: "features");

            //migrationBuilder.DropTable(
            //    name: "organization_subscriptions");

            //migrationBuilder.DropTable(
            //    name: "promotions");

            //migrationBuilder.DropTable(
            //    name: "organizations");

            //migrationBuilder.DropTable(
            //    name: "subscriptionplans");
        }
    }
}
