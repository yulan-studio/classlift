using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations;

public partial class ActivitySeriesCleanSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("Cost", "activities");
        migrationBuilder.DropColumn("MaxCapacity", "activities");
        migrationBuilder.DropColumn("ScheduledAt", "activities");
        migrationBuilder.DropColumn("ScheduledHours", "activities");
        migrationBuilder.DropColumn("ScheduledLocalTime", "activities");
        migrationBuilder.DropColumn("ScheduledTimeZoneId", "activities");
        migrationBuilder.DropColumn("Status", "activities");
        migrationBuilder.AddColumn<bool>("IsActive", "activities", type: "tinyint(1)", nullable: false, defaultValue: true);
        migrationBuilder.AddColumn<string>("ScheduleType", "activities", type: "varchar(40)", maxLength: 40, nullable: false, defaultValue: "OneTime");
        migrationBuilder.AddColumn<int>("ActivitySessionID", "activity_enrollments", type: "int", nullable: true);
        migrationBuilder.CreateTable("activity_sessions", table => new
        {
            ActivitySessionID = table.Column<int>(type: "int", nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            ActivityID = table.Column<int>(type: "int", nullable: false),
            ContactID = table.Column<int>(type: "int", nullable: true),
            ScheduledAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            ScheduledLocalTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            ScheduledTimeZoneId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
            ScheduledHours = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            MaxCapacity = table.Column<int>(type: "int", nullable: false),
            Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            Status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
            CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
        }, constraints: table => { table.PrimaryKey("PK_activity_sessions", x => x.ActivitySessionID); table.ForeignKey("FK_activity_sessions_activities_ActivityID", x => x.ActivityID, "activities", "ActivityID", onDelete: ReferentialAction.Cascade); table.ForeignKey("FK_activity_sessions_users_ContactID", x => x.ContactID, "users", "Id", onDelete: ReferentialAction.SetNull); });
        migrationBuilder.CreateIndex("IX_activity_enrollments_ActivitySessionID", "activity_enrollments", "ActivitySessionID");
        migrationBuilder.CreateIndex("IX_activity_sessions_ActivityID", "activity_sessions", "ActivityID");
        migrationBuilder.CreateIndex("IX_activity_sessions_ContactID", "activity_sessions", "ContactID");
        migrationBuilder.AddForeignKey(
            name: "FK_activity_enrollments_activity_sessions_ActivitySessionID",
            table: "activity_enrollments",
            column: "ActivitySessionID",
            principalTable: "activity_sessions",
            principalColumn: "ActivitySessionID",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_activity_enrollments_activity_sessions_ActivitySessionID",
            table: "activity_enrollments");
        migrationBuilder.DropTable("activity_sessions");
        migrationBuilder.DropIndex("IX_activity_enrollments_ActivitySessionID", "activity_enrollments");
        migrationBuilder.DropColumn("ActivitySessionID", "activity_enrollments");
        migrationBuilder.DropColumn("IsActive", "activities");
        migrationBuilder.DropColumn("ScheduleType", "activities");
    }
}
