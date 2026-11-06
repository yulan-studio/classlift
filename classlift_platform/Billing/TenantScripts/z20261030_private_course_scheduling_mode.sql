-- MySQL fallback script. Prefer applying the EF Core migration instead.
ALTER TABLE `courses`
    ADD COLUMN `PrivateSchedulingMode` varchar(20) NULL;

UPDATE `courses`
SET `PrivateSchedulingMode` = 'Coach'
WHERE `CourseType` = 'Private';
