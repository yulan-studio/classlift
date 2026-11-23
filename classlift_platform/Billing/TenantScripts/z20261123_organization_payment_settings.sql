-- Creates the organization-level Payment Settings table.
-- Run this once in each ClassLift Management tenant database.

CREATE TABLE IF NOT EXISTS `organization_payment_settings` (
    `OrganizationPaymentSettingsID` INT NOT NULL,
    `DirectPaymentInstructionTemplate` VARCHAR(2000)
        CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAtUtc` DATETIME(6) NOT NULL,
    `UpdatedAtUtc` DATETIME(6) NOT NULL,
    CONSTRAINT `PK_organization_payment_settings`
        PRIMARY KEY (`OrganizationPaymentSettingsID`)
) CHARACTER SET utf8mb4;