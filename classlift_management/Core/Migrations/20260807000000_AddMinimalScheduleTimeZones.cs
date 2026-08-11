using System;
using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260807000000_AddMinimalScheduleTimeZones")]
public partial class AddMinimalScheduleTimeZones : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "TimeZoneId",
            table: "users",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "America/Toronto")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<DateTime>(
            name: "ScheduledLocalTime",
            table: "activities",
            type: "datetime(6)",
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "ScheduledTimeZoneId",
            table: "activities",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<DateTime>(
            name: "ScheduledLocalTime",
            table: "course_enrollments",
            type: "datetime(6)",
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "ScheduledTimeZoneId",
            table: "course_enrollments",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "TimeZoneId", table: "users");
        migrationBuilder.DropColumn(name: "ScheduledLocalTime", table: "activities");
        migrationBuilder.DropColumn(name: "ScheduledTimeZoneId", table: "activities");
        migrationBuilder.DropColumn(name: "ScheduledLocalTime", table: "course_enrollments");
        migrationBuilder.DropColumn(name: "ScheduledTimeZoneId", table: "course_enrollments");
    }
}
