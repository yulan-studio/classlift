-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: classlift
-- ------------------------------------------------------
-- Server version	8.4.8

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20250309170601_RenameIdentityTables','8.0.2');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activities`
--

DROP TABLE IF EXISTS `activities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activities` (
  `ActivityID` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `MaxCapacity` int DEFAULT NULL,
  `ScheduledAt` datetime(6) NOT NULL,
  `ScheduledHours` decimal(4,2) DEFAULT NULL,
  `Cost` decimal(10,2) DEFAULT NULL,
  `Status` varchar(50) NOT NULL,
  `ContactID` int DEFAULT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`ActivityID`),
  KEY `IX_activities_ContactID` (`ContactID`),
  KEY `IX_activities_CreatedBy` (`CreatedBy`),
  KEY `IX_activities_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_activities_users_ContactID` FOREIGN KEY (`ContactID`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_activities_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_activities_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activities`
--

LOCK TABLES `activities` WRITE;
/*!40000 ALTER TABLE `activities` DISABLE KEYS */;
/*!40000 ALTER TABLE `activities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activity_enrollments`
--

DROP TABLE IF EXISTS `activity_enrollments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activity_enrollments` (
  `EnrollmentID` int NOT NULL AUTO_INCREMENT,
  `ChildID` int NOT NULL,
  `ActivityID` int NOT NULL,
  `Status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`EnrollmentID`),
  KEY `IX_activity_enrollments_ActivityID` (`ActivityID`),
  KEY `IX_activity_enrollments_ChildID` (`ChildID`),
  KEY `IX_activity_enrollments_CreatedBy` (`CreatedBy`),
  KEY `IX_activity_enrollments_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_activity_enrollments_activities_ActivityID` FOREIGN KEY (`ActivityID`) REFERENCES `activities` (`ActivityID`) ON DELETE CASCADE,
  CONSTRAINT `FK_activity_enrollments_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`) ON DELETE CASCADE,
  CONSTRAINT `FK_activity_enrollments_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_activity_enrollments_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activity_enrollments`
--

LOCK TABLES `activity_enrollments` WRITE;
/*!40000 ALTER TABLE `activity_enrollments` DISABLE KEYS */;
/*!40000 ALTER TABLE `activity_enrollments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activity_feedback`
--

DROP TABLE IF EXISTS `activity_feedback`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activity_feedback` (
  `FeedbackID` int NOT NULL AUTO_INCREMENT,
  `ChildID` int DEFAULT NULL,
  `ActivityID` int DEFAULT NULL,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`FeedbackID`),
  KEY `IX_activity_feedback_ActivityID` (`ActivityID`),
  KEY `IX_activity_feedback_ChildID` (`ChildID`),
  KEY `IX_activity_feedback_CreatedBy` (`CreatedBy`),
  KEY `IX_activity_feedback_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_activity_feedback_activities_ActivityID` FOREIGN KEY (`ActivityID`) REFERENCES `activities` (`ActivityID`),
  CONSTRAINT `FK_activity_feedback_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`),
  CONSTRAINT `FK_activity_feedback_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_activity_feedback_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activity_feedback`
--

LOCK TABLES `activity_feedback` WRITE;
/*!40000 ALTER TABLE `activity_feedback` DISABLE KEYS */;
/*!40000 ALTER TABLE `activity_feedback` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activity_notifications`
--

DROP TABLE IF EXISTS `activity_notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activity_notifications` (
  `NotificationID` int NOT NULL AUTO_INCREMENT,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ActivityID` int DEFAULT NULL,
  `EnrollmentID` int DEFAULT NULL,
  `ScheduledSend` datetime(6) DEFAULT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`NotificationID`),
  KEY `IX_ActivityNotifications_ActivityID` (`ActivityID`),
  KEY `IX_ActivityNotifications_CreatedBy` (`CreatedBy`),
  KEY `IX_ActivityNotifications_EnrollmentID` (`EnrollmentID`),
  KEY `IX_ActivityNotifications_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_ActivityNotifications_activities_ActivityID` FOREIGN KEY (`ActivityID`) REFERENCES `activities` (`ActivityID`),
  CONSTRAINT `FK_ActivityNotifications_activity_enrollments_EnrollmentID` FOREIGN KEY (`EnrollmentID`) REFERENCES `activity_enrollments` (`EnrollmentID`),
  CONSTRAINT `FK_ActivityNotifications_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActivityNotifications_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activity_notifications`
--

LOCK TABLES `activity_notifications` WRITE;
/*!40000 ALTER TABLE `activity_notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `activity_notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `admins`
--

DROP TABLE IF EXISTS `admins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `admins` (
  `AdminID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Wechat` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`AdminID`),
  UNIQUE KEY `IX_admins_UserID` (`UserID`),
  CONSTRAINT `FK_admins_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admins`
--

LOCK TABLES `admins` WRITE;
/*!40000 ALTER TABLE `admins` DISABLE KEYS */;
/*!40000 ALTER TABLE `admins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `child_balance`
--

DROP TABLE IF EXISTS `child_balance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `child_balance` (
  `BalanceID` int NOT NULL AUTO_INCREMENT,
  `ChildID` int DEFAULT NULL,
  `PaymentID` int DEFAULT NULL,
  `CourseID` int DEFAULT NULL,
  `ActivityID` int DEFAULT NULL,
  `EnrollmentID` int DEFAULT NULL,
  `Remarks` varchar(1000) DEFAULT NULL,
  `TransactionType` enum('Payment','Adjustment','Activity','Course','Course Session','Refund') NOT NULL,
  `Calculation` varchar(255) DEFAULT NULL,
  `BalanceChange` decimal(10,2) DEFAULT NULL,
  `Balance` decimal(10,2) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`BalanceID`),
  KEY `IX_child_balance_ActivityID` (`ActivityID`),
  KEY `IX_child_balance_ChildID` (`ChildID`),
  KEY `IX_child_balance_CourseID` (`CourseID`),
  KEY `IX_child_balance_EnrollmentID` (`EnrollmentID`),
  KEY `IX_child_balance_PaymentID` (`PaymentID`),
  CONSTRAINT `FK_child_balance_activities_ActivityID` FOREIGN KEY (`ActivityID`) REFERENCES `activities` (`ActivityID`),
  CONSTRAINT `FK_child_balance_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`),
  CONSTRAINT `FK_child_balance_course_enrollments_EnrollmentID` FOREIGN KEY (`EnrollmentID`) REFERENCES `course_enrollments` (`EnrollmentID`),
  CONSTRAINT `FK_child_balance_courses_CourseID` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`),
  CONSTRAINT `FK_child_balance_payments_PaymentID` FOREIGN KEY (`PaymentID`) REFERENCES `payments` (`PaymentID`)
) ENGINE=InnoDB AUTO_INCREMENT=326 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `child_balance`
--

LOCK TABLES `child_balance` WRITE;
/*!40000 ALTER TABLE `child_balance` DISABLE KEYS */;
/*!40000 ALTER TABLE `child_balance` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `children`
--

DROP TABLE IF EXISTS `children`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `children` (
  `ChildID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PreferedName` varchar(45) DEFAULT NULL,
  `BirthDate` datetime(6) DEFAULT NULL,
  `Gender` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CityID` int DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `HasOAP` tinyint(1) DEFAULT NULL,
  `OAPAmount` int DEFAULT NULL,
  `MemberID` varchar(10) DEFAULT NULL,
  `PrimaryDiagnosis` varchar(255) DEFAULT NULL,
  `PhotoConsent` tinyint NOT NULL DEFAULT '0',
  `Notes` text,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`ChildID`),
  UNIQUE KEY `IX_children_UserID` (`UserID`),
  KEY `IX_children_CityID` (`CityID`),
  KEY `FK_children_users_CreatedBy` (`CreatedBy`),
  KEY `FK_children_users_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_children_cities_CityID` FOREIGN KEY (`CityID`) REFERENCES `cities` (`CityID`),
  CONSTRAINT `FK_children_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_children_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_children_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=172 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `children`
--

LOCK TABLES `children` WRITE;
/*!40000 ALTER TABLE `children` DISABLE KEYS */;
/*!40000 ALTER TABLE `children` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cities`
--

DROP TABLE IF EXISTS `cities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cities` (
  `CityID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT NULL,
  `UpdatedDate` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`CityID`),
  KEY `IX_cities_CreatedBy` (`CreatedBy`),
  KEY `IX_cities_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_cities_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_cities_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cities`
--

LOCK TABLES `cities` WRITE;
/*!40000 ALTER TABLE `cities` DISABLE KEYS */;
/*!40000 ALTER TABLE `cities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coach_income`
--

DROP TABLE IF EXISTS `coach_income`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `coach_income` (
  `IncomeID` int NOT NULL AUTO_INCREMENT,
  `CoachID` int DEFAULT NULL,
  `CourseID` int DEFAULT NULL,
  `EnrollmentID` int DEFAULT NULL,
  `IncomeChange` decimal(10,2) DEFAULT NULL,
  `Income` decimal(10,2) DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IncomeID`),
  KEY `IX_coach_income_CoachID` (`CoachID`),
  KEY `IX_coach_income_CourseID` (`CourseID`),
  KEY `IX_coach_income_EnrollmentID` (`EnrollmentID`),
  CONSTRAINT `FK_coach_income_coaches_CoachID` FOREIGN KEY (`CoachID`) REFERENCES `coaches` (`CoachID`),
  CONSTRAINT `FK_coach_income_course_enrollments_EnrollmentID` FOREIGN KEY (`EnrollmentID`) REFERENCES `course_enrollments` (`EnrollmentID`),
  CONSTRAINT `FK_coach_income_courses_CourseID` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`)
) ENGINE=InnoDB AUTO_INCREMENT=252 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coach_income`
--

LOCK TABLES `coach_income` WRITE;
/*!40000 ALTER TABLE `coach_income` DISABLE KEYS */;
/*!40000 ALTER TABLE `coach_income` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coach_specialty`
--

DROP TABLE IF EXISTS `coach_specialty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `coach_specialty` (
  `CoachID` int NOT NULL,
  `SpecialtyID` int NOT NULL,
  KEY `coach_specialty_ibfk_1_idx` (`CoachID`),
  KEY `coach_specialty_ibfk_2` (`SpecialtyID`),
  CONSTRAINT `coach_specialty_ibfk_1` FOREIGN KEY (`CoachID`) REFERENCES `coaches` (`CoachID`) ON DELETE CASCADE,
  CONSTRAINT `coach_specialty_ibfk_2` FOREIGN KEY (`SpecialtyID`) REFERENCES `specialties` (`SpecialtyID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coach_specialty`
--

LOCK TABLES `coach_specialty` WRITE;
/*!40000 ALTER TABLE `coach_specialty` DISABLE KEYS */;
/*!40000 ALTER TABLE `coach_specialty` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coaches`
--

DROP TABLE IF EXISTS `coaches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `coaches` (
  `CoachID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `MemberID` varchar(10) DEFAULT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PreferedName` varchar(45) DEFAULT NULL,
  `Gender` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `BirthDate` datetime(6) DEFAULT NULL,
  `Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Wechat` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CityID` int NOT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `PostCode` varchar(10) DEFAULT NULL,
  `Bank` int DEFAULT NULL,
  `Transit` int DEFAULT NULL,
  `Account` int DEFAULT NULL,
  `Status` enum('Citizen','PR','WP','SP','Unknown') DEFAULT 'Unknown',
  `Avalibility` varchar(255) DEFAULT NULL,
  `PhotoConsent` tinyint(1) DEFAULT '1',
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`CoachID`),
  UNIQUE KEY `IX_coaches_UserID` (`UserID`),
  KEY `IX_coaches_CityID` (`CityID`),
  CONSTRAINT `FK_coaches_cities_CityID` FOREIGN KEY (`CityID`) REFERENCES `cities` (`CityID`) ON DELETE CASCADE,
  CONSTRAINT `FK_coaches_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=141 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coaches`
--

LOCK TABLES `coaches` WRITE;
/*!40000 ALTER TABLE `coaches` DISABLE KEYS */;
/*!40000 ALTER TABLE `coaches` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `course_enrollments`
--

DROP TABLE IF EXISTS `course_enrollments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `course_enrollments` (
  `EnrollmentID` int NOT NULL AUTO_INCREMENT,
  `ChildID` int DEFAULT NULL,
  `CourseID` int NOT NULL,
  `ScheduledAt` datetime(6) DEFAULT NULL,
  `ScheduledHours` decimal(4,2) DEFAULT NULL,
  `ActualHours` decimal(4,2) DEFAULT NULL,
  `Status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  `ParentNote` text,
  `StaffNote` text,
  `CoachNote` text,
  `Location` varchar(100) DEFAULT NULL,
  `EnrollmentID_Ref` int DEFAULT NULL,
  PRIMARY KEY (`EnrollmentID`),
  KEY `IX_course_enrollments_ChildID` (`ChildID`),
  KEY `IX_course_enrollments_CourseID` (`CourseID`),
  KEY `IX_course_enrollments_CreatedBy` (`CreatedBy`),
  KEY `IX_course_enrollments_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_course_enrollments_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`) ON DELETE CASCADE,
  CONSTRAINT `FK_course_enrollments_courses_CourseID` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`) ON DELETE CASCADE,
  CONSTRAINT `FK_course_enrollments_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_course_enrollments_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1116 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `course_enrollments`
--

LOCK TABLES `course_enrollments` WRITE;
/*!40000 ALTER TABLE `course_enrollments` DISABLE KEYS */;
/*!40000 ALTER TABLE `course_enrollments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `course_notifications`
--

DROP TABLE IF EXISTS `course_notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `course_notifications` (
  `NotificationID` int NOT NULL AUTO_INCREMENT,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CourseID` int DEFAULT NULL,
  `EnrollmentID` int DEFAULT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  `CreatedByUserId` int NOT NULL,
  `UpdatedByUserId` int NOT NULL,
  PRIMARY KEY (`NotificationID`),
  KEY `IX_course_notifications_CourseID` (`CourseID`),
  KEY `IX_course_notifications_CreatedByUserId` (`CreatedByUserId`),
  KEY `IX_course_notifications_EnrollmentID` (`EnrollmentID`),
  KEY `IX_course_notifications_UpdatedByUserId` (`UpdatedByUserId`),
  CONSTRAINT `FK_course_notifications_course_enrollments_EnrollmentID` FOREIGN KEY (`EnrollmentID`) REFERENCES `course_enrollments` (`EnrollmentID`),
  CONSTRAINT `FK_course_notifications_courses_CourseID` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`),
  CONSTRAINT `FK_course_notifications_users_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_course_notifications_users_UpdatedByUserId` FOREIGN KEY (`UpdatedByUserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `course_notifications`
--

LOCK TABLES `course_notifications` WRITE;
/*!40000 ALTER TABLE `course_notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `course_notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `courses`
--

DROP TABLE IF EXISTS `courses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `courses` (
  `CourseID` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CourseType` varchar(45) DEFAULT NULL,
  `MaxCapacity` int DEFAULT NULL,
  `SessionCount` int DEFAULT NULL,
  `HourlyCost` decimal(10,2) NOT NULL,
  `HourlyCost2` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CoachID` int DEFAULT NULL,
  `SpecialtyID` int NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`CourseID`),
  KEY `IX_courses_CoachID` (`CoachID`),
  KEY `IX_courses_CreatedBy` (`CreatedBy`),
  KEY `IX_courses_UpdatedBy` (`UpdatedBy`),
  KEY `FK_courses_specialties_SpecialtyID` (`SpecialtyID`),
  CONSTRAINT `FK_courses_coaches_CoachID` FOREIGN KEY (`CoachID`) REFERENCES `coaches` (`CoachID`) ON DELETE CASCADE,
  CONSTRAINT `FK_courses_specialties_SpecialtyID` FOREIGN KEY (`SpecialtyID`) REFERENCES `specialties` (`SpecialtyID`),
  CONSTRAINT `FK_courses_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_courses_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `courses`
--

LOCK TABLES `courses` WRITE;
/*!40000 ALTER TABLE `courses` DISABLE KEYS */;
/*!40000 ALTER TABLE `courses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emergency_contacts`
--

DROP TABLE IF EXISTS `emergency_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emergency_contacts` (
  `EmergencyContactID` int NOT NULL AUTO_INCREMENT,
  `ChildID` int DEFAULT NULL,
  `CoachID` int DEFAULT NULL,
  `ContactName` varchar(100) NOT NULL,
  `Relationship` varchar(50) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`EmergencyContactID`),
  KEY `emergency_contacts_ibfk_2_idx` (`CoachID`),
  KEY `emergency_contacts_ibfk_1` (`ChildID`),
  CONSTRAINT `emergency_contacts_ibfk_1` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`) ON DELETE CASCADE,
  CONSTRAINT `emergency_contacts_ibfk_2` FOREIGN KEY (`CoachID`) REFERENCES `coaches` (`CoachID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emergency_contacts`
--

LOCK TABLES `emergency_contacts` WRITE;
/*!40000 ALTER TABLE `emergency_contacts` DISABLE KEYS */;
/*!40000 ALTER TABLE `emergency_contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fees`
--

DROP TABLE IF EXISTS `fees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fees` (
  `FeeID` int NOT NULL AUTO_INCREMENT,
  `CourseEnrollmentID` int DEFAULT NULL,
  `ActivityEnrollmentID` int DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `PaymentModel` enum('Direct','Token','OAP','') NOT NULL DEFAULT '',
  `TotalCost` decimal(10,2) DEFAULT NULL,
  `IsPaid` tinyint(1) DEFAULT '0',
  `PaidAt` datetime DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`FeeID`),
  KEY `fk_fee_course` (`CourseEnrollmentID`),
  KEY `fk_fee_activity` (`ActivityEnrollmentID`),
  KEY `FK_fee_CreatedBy` (`CreatedBy`),
  KEY `FK_fee_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `fk_fee_activity` FOREIGN KEY (`ActivityEnrollmentID`) REFERENCES `activity_enrollments` (`EnrollmentID`),
  CONSTRAINT `fk_fee_course` FOREIGN KEY (`CourseEnrollmentID`) REFERENCES `course_enrollments` (`EnrollmentID`),
  CONSTRAINT `FK_fee_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_fee_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `chk_only_one` CHECK ((((`CourseEnrollmentID` is not null) and (`ActivityEnrollmentID` is null)) or ((`CourseEnrollmentID` is null) and (`ActivityEnrollmentID` is not null))))
) ENGINE=InnoDB AUTO_INCREMENT=76 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fees`
--

LOCK TABLES `fees` WRITE;
/*!40000 ALTER TABLE `fees` DISABLE KEYS */;
/*!40000 ALTER TABLE `fees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `parent_child`
--

DROP TABLE IF EXISTS `parent_child`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `parent_child` (
  `ParentChildID` int NOT NULL AUTO_INCREMENT,
  `ParentID` int NOT NULL,
  `ChildID` int NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  `Relationship` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`ParentChildID`),
  KEY `IX_parent_child_ChildID` (`ChildID`),
  KEY `IX_parent_child_ParentID` (`ParentID`),
  CONSTRAINT `FK_parent_child_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`) ON DELETE CASCADE,
  CONSTRAINT `FK_parent_child_parents_ParentID` FOREIGN KEY (`ParentID`) REFERENCES `parents` (`ParentID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parent_child`
--

LOCK TABLES `parent_child` WRITE;
/*!40000 ALTER TABLE `parent_child` DISABLE KEYS */;
/*!40000 ALTER TABLE `parent_child` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `parents`
--

DROP TABLE IF EXISTS `parents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `parents` (
  `ParentID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Wechat` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`ParentID`),
  KEY `IX_parents_CreatedBy` (`CreatedBy`),
  KEY `IX_parents_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_parents_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_parents_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parents`
--

LOCK TABLES `parents` WRITE;
/*!40000 ALTER TABLE `parents` DISABLE KEYS */;
/*!40000 ALTER TABLE `parents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_package`
--

DROP TABLE IF EXISTS `payment_package`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_package` (
  `PackageID` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`PackageID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_package`
--

LOCK TABLES `payment_package` WRITE;
/*!40000 ALTER TABLE `payment_package` DISABLE KEYS */;
/*!40000 ALTER TABLE `payment_package` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `PaymentID` int NOT NULL AUTO_INCREMENT,
  `ParentID` int NOT NULL,
  `PaymentPackageID` int DEFAULT NULL,
  `FeeID` int DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `Receipt` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PaymentDate` datetime(6) DEFAULT NULL,
  `Memo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `ChildID` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`PaymentID`),
  KEY `IX_payments_ChildID` (`ChildID`),
  KEY `IX_payments_CreatedBy` (`CreatedBy`),
  KEY `IX_payments_ParentID` (`ParentID`),
  KEY `IX_payments_PaymentPackageID` (`PaymentPackageID`),
  KEY `IX_payments_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_payments_children_ChildID` FOREIGN KEY (`ChildID`) REFERENCES `children` (`ChildID`) ON DELETE CASCADE,
  CONSTRAINT `FK_payments_parents_ParentID` FOREIGN KEY (`ParentID`) REFERENCES `parents` (`ParentID`) ON DELETE CASCADE,
  CONSTRAINT `FK_payments_payment_package_PaymentPackageID` FOREIGN KEY (`PaymentPackageID`) REFERENCES `payment_package` (`PackageID`),
  CONSTRAINT `FK_payments_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_payments_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roleclaims`
--

DROP TABLE IF EXISTS `roleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_roleclaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_roleclaims_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roleclaims`
--

LOCK TABLES `roleclaims` WRITE;
/*!40000 ALTER TABLE `roleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `roleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `specialties`
--

DROP TABLE IF EXISTS `specialties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `specialties` (
  `SpecialtyID` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int NOT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT NULL,
  `UpdatedDate` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`SpecialtyID`),
  KEY `IX_specialties_CreatedBy` (`CreatedBy`),
  KEY `IX_specialties_UpdatedBy` (`UpdatedBy`),
  CONSTRAINT `FK_specialties_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_specialties_users_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `specialties`
--

LOCK TABLES `specialties` WRITE;
/*!40000 ALTER TABLE `specialties` DISABLE KEYS */;
/*!40000 ALTER TABLE `specialties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staff`
--

DROP TABLE IF EXISTS `staff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staff` (
  `StaffID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Wechat` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`StaffID`),
  UNIQUE KEY `IX_staff_UserID` (`UserID`),
  CONSTRAINT `FK_staff_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
/*!40000 ALTER TABLE `staff` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userclaims`
--

DROP TABLE IF EXISTS `userclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_userclaims_UserId` (`UserId`),
  CONSTRAINT `FK_userclaims_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userclaims`
--

LOCK TABLES `userclaims` WRITE;
/*!40000 ALTER TABLE `userclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `userclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userlogins`
--

DROP TABLE IF EXISTS `userlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` int NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_userlogins_UserId` (`UserId`),
  CONSTRAINT `FK_userlogins_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userlogins`
--

LOCK TABLES `userlogins` WRITE;
/*!40000 ALTER TABLE `userlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `userlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userroles`
--

DROP TABLE IF EXISTS `userroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userroles` (
  `UserId` int NOT NULL,
  `RoleId` int NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_userroles_RoleId` (`RoleId`),
  CONSTRAINT `FK_userroles_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_userroles_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userroles`
--

LOCK TABLES `userroles` WRITE;
/*!40000 ALTER TABLE `userroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `userroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `CreatedDate` datetime(6) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB AUTO_INCREMENT=339 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usertokens`
--

DROP TABLE IF EXISTS `usertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usertokens` (
  `UserId` int NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_usertokens_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usertokens`
--

LOCK TABLES `usertokens` WRITE;
/*!40000 ALTER TABLE `usertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `usertokens` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-26 23:44:51
