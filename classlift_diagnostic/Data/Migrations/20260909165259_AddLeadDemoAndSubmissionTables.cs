using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLift.Diagnostic.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadDemoAndSubmissionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE `leads` (`Id` char(36) NOT NULL, `CreatedAt` datetime(6) NOT NULL, `UpdatedAt` datetime(6) NOT NULL, `Name` varchar(120) NOT NULL, `Email` varchar(320) NOT NULL, `Organization` varchar(200) NULL, `WebsiteUrl` varchar(2048) NULL, `Phone` varchar(50) NULL, `Status` varchar(30) NOT NULL, PRIMARY KEY (`Id`), UNIQUE KEY `IX_leads_Email` (`Email`)) CHARACTER SET=utf8mb4;
                INSERT INTO `leads` (`Id`,`CreatedAt`,`UpdatedAt`,`Name`,`Email`,`Organization`,`WebsiteUrl`,`Phone`,`Status`) SELECT UUID(), MIN(`CreatedAt`), MAX(`CreatedAt`), MAX(`Name`), LOWER(`Email`), MAX(`Organization`), MAX(`WebsiteUrl`), NULL, 'NEW' FROM `diagnostic_leads` GROUP BY LOWER(`Email`);
                RENAME TABLE `diagnostic_leads` TO `diagnostic_submissions`;
                ALTER TABLE `diagnostic_submissions` ADD COLUMN `LeadId` char(36) NULL;
                UPDATE `diagnostic_submissions` s JOIN `leads` l ON l.`Email` = LOWER(s.`Email`) SET s.`LeadId` = l.`Id`;
                ALTER TABLE `diagnostic_submissions` MODIFY COLUMN `LeadId` char(36) NOT NULL;
                CREATE INDEX `IX_diagnostic_submissions_LeadId` ON `diagnostic_submissions` (`LeadId`);
                ALTER TABLE `diagnostic_submissions` ADD CONSTRAINT `FK_diagnostic_submissions_leads_LeadId` FOREIGN KEY (`LeadId`) REFERENCES `leads` (`Id`) ON DELETE CASCADE;
                CREATE TABLE `demo_requests` (`Id` char(36) NOT NULL, `LeadId` char(36) NOT NULL, `CreatedAt` datetime(6) NOT NULL, `Phone` varchar(50) NULL, `PreferredTime` varchar(120) NULL, `TimeZone` varchar(80) NULL, `CompanySize` varchar(80) NULL, `MainGoal` longtext NOT NULL, `CurrentSystem` longtext NULL, `Message` longtext NULL, `Source` varchar(50) NOT NULL, `Status` varchar(30) NOT NULL, PRIMARY KEY (`Id`), KEY `IX_demo_requests_CreatedAt` (`CreatedAt`), KEY `IX_demo_requests_Status` (`Status`), CONSTRAINT `FK_demo_requests_leads_LeadId` FOREIGN KEY (`LeadId`) REFERENCES `leads` (`Id`) ON DELETE CASCADE) CHARACTER SET=utf8mb4;
                """);
            migrationBuilder.Sql("ALTER TABLE `diagnostic_submissions` ADD COLUMN `UserReportJson` JSON NULL, ADD COLUMN `SalesReportJson` JSON NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `demo_requests`; RENAME TABLE `diagnostic_submissions` TO `diagnostic_leads`; DROP TABLE IF EXISTS `leads`;" );
        }
    }
}
