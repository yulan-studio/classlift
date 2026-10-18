-- Adds the Student + Private Course scheduling preference fields.
-- Run once in each affected tenant database.

ALTER TABLE `course_enrollments`
    ADD COLUMN `SchedulePreference` VARCHAR(2000) NULL,
    ADD COLUMN `SchedulePreferenceUpdatedAt` DATETIME(6) NULL;
