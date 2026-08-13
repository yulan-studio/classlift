-- Allow the application to store the supported coach status values.
ALTER TABLE `coaches`
    MODIFY COLUMN `Status` VARCHAR(20) NULL;
