-- ClassLift migration: AddActivityEnrollmentToChildBalance
-- Adds the activity enrollment reference used to display Activity Session Scheduled At
-- in the participant My Balance page.

ALTER TABLE `child_balance`
    ADD COLUMN `ActivityEnrollmentID` int NULL;

CREATE INDEX `IX_child_balance_ActivityEnrollmentID`
    ON `child_balance` (`ActivityEnrollmentID`);

ALTER TABLE `child_balance`
    ADD CONSTRAINT `FK_child_balance_activity_enrollments_ActivityEnrollmentID`
    FOREIGN KEY (`ActivityEnrollmentID`)
    REFERENCES `activity_enrollments` (`EnrollmentID`);
