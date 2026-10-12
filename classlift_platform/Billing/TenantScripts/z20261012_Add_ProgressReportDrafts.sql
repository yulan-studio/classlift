CREATE TABLE `ProgressReportDrafts` (
    `ProgressReportDraftId` int NOT NULL AUTO_INCREMENT,
    `SessionEnrollmentId` int NOT NULL,
    `CoachId` int NOT NULL,
    `WorkedOn` text NOT NULL,
    `PositiveResponse` text NOT NULL,
    `Difficulty` text NOT NULL,
    `ContinueNextTime` text NOT NULL,
    `GeneratedDraft` text NULL,
    `EditedDraft` text NULL,
    `Status` varchar(20) NOT NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `ApprovedAtUtc` datetime(6) NULL,
    `ModelName` longtext NULL,
    `PromptVersion` longtext NULL,
    CONSTRAINT `PK_ProgressReportDrafts`
        PRIMARY KEY (`ProgressReportDraftId`)
)
CHARACTER SET = utf8mb4;