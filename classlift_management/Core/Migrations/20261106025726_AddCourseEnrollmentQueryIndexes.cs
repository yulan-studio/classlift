using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEnrollmentQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_course_enrollments_Child_Status_Ref_ScheduledAt",
                table: "course_enrollments",
                columns: new[] { "ChildID", "Status", "EnrollmentID_Ref", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_enrollments_Ref_Status_ScheduledAt",
                table: "course_enrollments",
                columns: new[] { "EnrollmentID_Ref", "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_enrollments_Course_Child_Status_Ref_ScheduledAt",
                table: "course_enrollments",
                columns: new[] { "CourseID", "ChildID", "Status", "EnrollmentID_Ref", "ScheduledAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_course_enrollments_Child_Status_Ref_ScheduledAt",
                table: "course_enrollments");

            migrationBuilder.DropIndex(
                name: "IX_course_enrollments_Ref_Status_ScheduledAt",
                table: "course_enrollments");

            migrationBuilder.DropIndex(
                name: "IX_course_enrollments_Course_Child_Status_Ref_ScheduledAt",
                table: "course_enrollments");
        }
    }
}
