-- ClassLift / MySQL
-- Migration: 20261121204501_RequireExplicitActivityActiveState
-- Removes the database default so IsActive must be supplied explicitly.

ALTER TABLE `activities`
    MODIFY COLUMN `IsActive` tinyint(1) NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261121204501_RequireExplicitActivityActiveState', '8.0.2');
