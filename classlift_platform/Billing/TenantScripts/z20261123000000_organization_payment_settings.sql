-- Creates the organization-level Payment Settings table.
-- Run this once in each ClassLift Management tenant database.

CREATE TABLE IF NOT EXISTS `organization_payment_settings` (
    `OrganizationPaymentSettingsID` int NOT NULL,
    `DirectPaymentInstructionTemplate` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `UpdatedAtUtc` datetime(6) NOT NULL,
    CONSTRAINT `PK_organization_payment_settings`
        PRIMARY KEY (`OrganizationPaymentSettingsID`)
) CHARACTER SET utf8mb4;

INSERT INTO `organization_payment_settings`
(
    `OrganizationPaymentSettingsID`,
    `DirectPaymentInstructionTemplate`,
    `CreatedAtUtc`,
    `UpdatedAtUtc`
)
SELECT
    1,
    'Please pay {amount} for {payment_scope} using the payment method selected by the organization.',
    UTC_TIMESTAMP(6),
    UTC_TIMESTAMP(6)
WHERE NOT EXISTS (
    SELECT 1
    FROM `organization_payment_settings`
    WHERE `OrganizationPaymentSettingsID` = 1
);
