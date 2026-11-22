-- ClassLift / MySQL
-- Migration: 20261120170000_ActivitySeriesCleanSchema
-- Review the target database before running this script.

ALTER TABLE `activities`
    DROP COLUMN `Cost`,
    DROP COLUMN `MaxCapacity`,
    DROP COLUMN `ScheduledAt`,
    DROP COLUMN `ScheduledHours`,
    DROP COLUMN `ScheduledLocalTime`,
    DROP COLUMN `ScheduledTimeZoneId`,
    DROP COLUMN `Status`;

ALTER TABLE `activities`
    ADD COLUMN `IsActive` tinyint(1) NOT NULL DEFAULT 1,
    ADD COLUMN `ScheduleType` varchar(40) NOT NULL DEFAULT 'OneTime';

ALTER TABLE `activity_enrollments`
    ADD COLUMN `ActivitySessionID` int NULL;

CREATE TABLE `activity_sessions` (
    `ActivitySessionID` int NOT NULL AUTO_INCREMENT,
    `ActivityID` int NOT NULL,
    `ContactID` int NULL,
    `ScheduledAt` datetime(6) NOT NULL,
    `ScheduledLocalTime` datetime(6) NULL,
    `ScheduledTimeZoneId` varchar(100) NULL,
    `ScheduledHours` decimal(65,30) NOT NULL,
    `MaxCapacity` int NOT NULL,
    `Cost` decimal(10,2) NOT NULL,
    `Status` varchar(40) NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    `UpdatedDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_activity_sessions` PRIMARY KEY (`ActivitySessionID`),
    CONSTRAINT `FK_activity_sessions_activities_ActivityID`
        FOREIGN KEY (`ActivityID`) REFERENCES `activities` (`ActivityID`) ON DELETE CASCADE,
    CONSTRAINT `FK_activity_sessions_users_ContactID`
        FOREIGN KEY (`ContactID`) REFERENCES `users` (`Id`) ON DELETE SET NULL
);

CREATE INDEX `IX_activity_enrollments_ActivitySessionID`
    ON `activity_enrollments` (`ActivitySessionID`);
CREATE INDEX `IX_activity_sessions_ActivityID`
    ON `activity_sessions` (`ActivityID`);
CREATE INDEX `IX_activity_sessions_ContactID`
    ON `activity_sessions` (`ContactID`);

ALTER TABLE `activity_enrollments`
    ADD CONSTRAINT `FK_activity_enrollments_activity_sessions_ActivitySessionID`
    FOREIGN KEY (`ActivitySessionID`) REFERENCES `activity_sessions` (`ActivitySessionID`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261120170000_ActivitySeriesCleanSchema', '8.0.2');
