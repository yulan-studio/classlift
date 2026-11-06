START TRANSACTION;

CREATE INDEX `IX_course_enrollments_Child_Status_Ref_ScheduledAt` ON `course_enrollments` (`ChildID`, `Status`, `EnrollmentID_Ref`, `ScheduledAt`);

CREATE INDEX `IX_course_enrollments_Ref_Status_ScheduledAt` ON `course_enrollments` (`EnrollmentID_Ref`, `Status`, `ScheduledAt`);

CREATE INDEX `IX_course_enrollments_Course_Child_Status_Ref_ScheduledAt` ON `course_enrollments` (`CourseID`, `ChildID`, `Status`, `EnrollmentID_Ref`, `ScheduledAt`);



COMMIT;

