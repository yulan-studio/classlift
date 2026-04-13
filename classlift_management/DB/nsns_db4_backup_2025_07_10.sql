-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: nsns_db4
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
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
  `ScheduledAt` datetime(6) DEFAULT NULL,
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
INSERT INTO `activities` VALUES (3,'Movie night','Disney Movie: Lion King','123 New Street, Brampton',40,'2025-03-25 20:00:00.000000',30.00,'Completed',NULL,13,13,'0001-01-01 00:00:00.000000','2025-03-21 23:21:19.334157'),(4,'Movie night','Movie night','123 Winston Churchill Rd, Mississauga',40,'2025-03-28 19:30:00.000000',20.00,'Completed',NULL,13,13,'2025-03-26 23:56:23.285735','2025-04-01 16:39:54.524641'),(5,'Basketball game','Basketball game, age 10-16','123 Dundas St, Mississauga',10,'2025-04-02 16:00:00.000000',20.00,'Completed',NULL,13,13,'2025-04-01 14:42:29.757272','2025-04-01 14:52:42.501972'),(6,'weekend camp','2 days event','123 King St, Toronto',5,'2025-04-19 15:21:00.000000',100.00,'Completed',NULL,13,13,'2025-04-01 15:23:54.150984','2025-04-02 11:51:44.461438'),(7,'summer camp','summer camp','123 Derry Rd',5,'2025-04-09 15:19:00.000000',100.00,'Completed',NULL,13,13,'2025-04-02 15:19:26.416827','2025-04-03 01:12:06.941625'),(8,'Skating event','Skating event','123 Queen St, Toronto',50,'2025-04-10 09:00:00.000000',30.00,'Completed',NULL,13,13,'2025-04-03 06:56:50.815684','2025-04-05 08:56:17.360465'),(9,'summer camp in 2025','summer camp in 2025','1234 Queen St, Toronto',10,'2025-04-09 10:00:00.000000',1500.00,'Completed',NULL,13,13,'2025-04-07 09:51:44.643217','2025-04-11 13:19:41.176654'),(10,'New activity for testing','New activity for testing\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nNew activity for testing\r\n\r\n','123 Derry Rd',10,'2025-04-09 14:30:00.000000',500.00,'Completed',NULL,13,NULL,'2025-04-11 13:39:36.064564','0001-01-01 00:00:00.000000'),(11,'New Activity for testing 2','New Activity for testing 2','1234 Queen St, Toronto',50,'2025-04-09 14:01:00.000000',1500.00,'Completed',NULL,13,NULL,'2025-04-11 14:01:20.641895','0001-01-01 00:00:00.000000');
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
INSERT INTO `activity_enrollments` VALUES (3,1,3,'Completed',13,NULL,'2025-03-22 03:55:42.666119','0001-01-01 00:00:00.000000'),(4,1,4,'Completed',13,NULL,'2025-03-27 04:03:57.183892','0001-01-01 00:00:00.000000'),(5,4,3,'Completed',13,NULL,'2025-04-01 15:26:07.858649','0001-01-01 00:00:00.000000'),(6,4,6,'Canceled',13,NULL,'2025-04-01 19:41:13.942371','0001-01-01 00:00:00.000000'),(7,2,6,'Canceled',13,NULL,'2025-04-02 15:51:20.242260','0001-01-01 00:00:00.000000'),(8,1,7,'Canceled',13,NULL,'2025-04-02 19:35:29.011076','0001-01-01 00:00:00.000000'),(9,2,7,'Canceled',13,NULL,'2025-04-02 19:36:20.537693','0001-01-01 00:00:00.000000'),(10,4,7,'Canceled',13,NULL,'2025-04-02 19:36:39.232364','0001-01-01 00:00:00.000000'),(11,5,7,'Canceled',13,NULL,'2025-04-03 02:56:31.717007','0001-01-01 00:00:00.000000'),(12,6,7,'Canceled',13,NULL,'2025-04-03 02:56:59.644933','0001-01-01 00:00:00.000000'),(13,4,8,'Completed',13,NULL,'2025-04-03 23:26:56.901221','0001-01-01 00:00:00.000000'),(15,1,8,'Completed',13,NULL,'2025-04-05 12:59:17.044667','0001-01-01 00:00:00.000000'),(16,8,9,'Completed',13,NULL,'2025-04-07 13:52:37.008880','0001-01-01 00:00:00.000000'),(17,4,9,'Completed',13,NULL,'2025-04-11 17:11:22.850405','0001-01-01 00:00:00.000000'),(18,4,10,'Completed',13,NULL,'2025-04-11 17:40:21.162976','0001-01-01 00:00:00.000000'),(19,4,11,'Completed',13,NULL,'2025-04-11 18:01:35.327305','0001-01-01 00:00:00.000000');
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
INSERT INTO `admins` VALUES (6,32,'Admin1','345618363','admin1_wechat'),(7,33,'Admin2','68690353','admin2_wechat');
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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `child_balance`
--

LOCK TABLES `child_balance` WRITE;
/*!40000 ALTER TABLE `child_balance` DISABLE KEYS */;
INSERT INTO `child_balance` VALUES (5,8,NULL,17,NULL,106,-120.00,-120.00,'2025-07-06 12:28:32.519650',22,22,'2025-07-06 12:28:32.519691'),(6,8,NULL,17,NULL,105,-240.00,-360.00,'2025-07-06 12:28:41.364248',22,22,'2025-07-06 12:28:41.364249');
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
  `BirthDate` datetime(6) DEFAULT NULL,
  `Gender` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CityID` int DEFAULT NULL,
  `HasOAP` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ChildID`),
  UNIQUE KEY `IX_children_UserID` (`UserID`),
  KEY `IX_children_CityID` (`CityID`),
  CONSTRAINT `FK_children_cities_CityID` FOREIGN KEY (`CityID`) REFERENCES `cities` (`CityID`),
  CONSTRAINT `FK_children_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `children`
--

LOCK TABLES `children` WRITE;
/*!40000 ALTER TABLE `children` DISABLE KEYS */;
INSERT INTO `children` VALUES (1,16,'JJ','2014-01-11 00:00:00.000000','Male',2,0),(2,17,'CC','2024-02-13 00:00:00.000000','Female',3,1),(4,24,'child1','2017-01-10 00:00:00.000000','Male',3,1),(5,25,'child2','2016-01-02 00:00:00.000000','Female',3,1),(6,26,'child3','2011-02-09 00:00:00.000000','Male',4,1),(8,31,'child4','2014-01-08 00:00:00.000000','Female',3,1);
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cities`
--

LOCK TABLES `cities` WRITE;
/*!40000 ALTER TABLE `cities` DISABLE KEYS */;
INSERT INTO `cities` VALUES (2,'Oakville',13,NULL,'2025-03-18 20:14:42','2025-03-18 20:14:15'),(3,'Mississauga',13,NULL,'2025-03-18 16:18:14','2025-03-18 20:18:12'),(4,'Vaughan',13,NULL,'2025-03-18 16:34:34',NULL),(5,'Brampton',13,NULL,'2025-04-04 19:33:18',NULL),(6,'Toronto',13,NULL,'2025-04-04 19:33:50',NULL),(7,'Scarborough',13,NULL,'2025-04-04 19:36:59',NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coach_income`
--

LOCK TABLES `coach_income` WRITE;
/*!40000 ALTER TABLE `coach_income` DISABLE KEYS */;
INSERT INTO `coach_income` VALUES (4,5,17,106,120.00,120.00,'2025-07-06 12:28:32.453633',22,NULL,NULL),(5,5,17,105,240.00,360.00,'2025-07-06 12:28:41.358732',22,NULL,NULL);
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
INSERT INTO `coach_specialty` VALUES (2,2),(2,3),(4,2),(5,5),(6,4),(7,6),(7,7),(8,2),(8,4);
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
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gender` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Wechat` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CityID` int NOT NULL,
  `Avalibility` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`CoachID`),
  UNIQUE KEY `IX_coaches_UserID` (`UserID`),
  KEY `IX_coaches_CityID` (`CityID`),
  CONSTRAINT `FK_coaches_cities_CityID` FOREIGN KEY (`CityID`) REFERENCES `cities` (`CityID`) ON DELETE CASCADE,
  CONSTRAINT `FK_coaches_users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coaches`
--

LOCK TABLES `coaches` WRITE;
/*!40000 ALTER TABLE `coaches` DISABLE KEYS */;
INSERT INTO `coaches` VALUES (2,19,'coach2','Male','6475555555','coach2_wechat',3,NULL),(4,21,'coach3','Female','6474422443','coach3_wechat',3,NULL),(5,22,'coach4','Female','6475522443','coach4_wechat',2,NULL),(6,29,'coach5','Female','6475555555','coach5_wechat',3,NULL),(7,30,'coach6','Male','6475555555','coach6_wechat',5,NULL),(8,34,'coach_2025','Male','3214321431','coach_2025_wechat',5,NULL);
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
  `ScheduledHours` decimal(4,1) DEFAULT NULL,
  `ActualHours` decimal(4,1) DEFAULT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=140 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `course_enrollments`
--

LOCK TABLES `course_enrollments` WRITE;
/*!40000 ALTER TABLE `course_enrollments` DISABLE KEYS */;
INSERT INTO `course_enrollments` VALUES (11,1,7,NULL,0.0,NULL,'Registered',13,NULL,'2025-03-29 00:52:27.274733','2025-03-29 00:52:27.274623',NULL,NULL,NULL,NULL,NULL),(12,1,5,NULL,0.0,NULL,'Registered',13,NULL,'2025-03-29 00:53:01.111151','2025-03-29 00:53:01.111149',NULL,NULL,NULL,NULL,NULL),(29,2,5,NULL,0.0,NULL,'Registered',13,NULL,'2025-04-03 00:13:51.194901','2025-04-03 00:13:51.194752',NULL,NULL,NULL,NULL,NULL),(30,4,5,NULL,0.0,NULL,'Registered',13,NULL,'2025-04-03 00:14:24.542468','2025-04-03 00:14:24.542466',NULL,NULL,NULL,NULL,NULL),(31,4,6,NULL,0.0,NULL,'Registered',13,NULL,'2025-04-03 00:58:59.295310','2025-04-03 00:58:59.295200',NULL,NULL,NULL,NULL,NULL),(41,8,11,NULL,0.0,NULL,'Registered',13,NULL,'2025-04-30 10:04:10.689287','2025-04-30 10:04:10.688909',NULL,NULL,NULL,NULL,NULL),(42,1,11,NULL,0.0,NULL,'Registered',13,NULL,'2025-06-06 16:12:46.314590','2025-06-06 16:12:46.314375',NULL,NULL,NULL,NULL,NULL),(43,8,17,NULL,0.0,NULL,'Registered',13,NULL,'2025-06-09 15:54:41.293717','2025-06-09 15:54:41.293449',NULL,NULL,NULL,NULL,NULL),(50,NULL,13,'2025-06-12 09:00:00.000000',1.0,NULL,'Canceled',13,NULL,'2025-06-11 11:18:42.815845','2025-06-11 11:18:42.815685',NULL,'Note 2',NULL,'community center 1',NULL),(52,NULL,13,'2025-06-13 09:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-12 16:47:13.159225','2025-06-12 16:47:13.158840',NULL,NULL,NULL,'community center 1',NULL),(53,NULL,13,'2025-06-20 09:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-13 14:34:31.894591','2025-06-13 14:34:31.894338',NULL,NULL,NULL,'community center 1',NULL),(54,NULL,13,'2025-06-27 09:59:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-13 14:35:11.962501','2025-06-13 14:35:11.962498',NULL,NULL,NULL,'community center 1',NULL),(58,8,13,NULL,0.0,NULL,'Registered',13,NULL,'2025-06-15 11:56:50.955103','2025-06-15 11:56:50.954737',NULL,NULL,NULL,NULL,NULL),(59,8,13,'2025-06-13 09:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-15 12:21:42.198047','2025-07-02 03:15:03.254098',NULL,NULL,NULL,NULL,52),(77,8,17,'2025-06-09 07:30:00.000000',2.0,NULL,'Deleted',22,NULL,'2025-06-15 16:29:56.650556','2025-06-15 16:29:56.650395','Can you please reschedule?',NULL,NULL,NULL,43),(79,8,17,'2025-06-23 09:30:00.000000',1.0,NULL,'Deleted',22,NULL,'2025-06-15 16:31:05.769891','2025-06-15 16:31:05.769890','Please reschedule to July 4.',NULL,NULL,NULL,43),(80,8,13,'2025-06-20 09:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-16 13:28:19.572128','2025-07-02 03:15:03.303465',NULL,NULL,NULL,NULL,53),(81,8,13,'2025-06-27 09:59:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-16 13:28:19.698227','2025-07-02 03:15:03.316453',NULL,NULL,NULL,NULL,54),(83,NULL,13,'2025-07-08 14:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-30 23:07:55.427672','2025-06-30 23:07:55.427174',NULL,NULL,NULL,'community center 1',NULL),(84,NULL,13,'2025-06-12 14:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-06-30 23:08:23.648324','2025-06-30 23:08:23.648319',NULL,NULL,NULL,'community center 1',NULL),(85,NULL,13,'2025-07-11 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-06-30 23:09:37.780227','2025-06-30 23:09:37.780219',NULL,NULL,NULL,'community center 1',NULL),(86,8,13,'2025-07-08 14:00:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-01 00:59:57.093479','2025-07-02 03:15:03.330024',NULL,NULL,NULL,NULL,83),(87,8,13,'2025-07-11 14:00:00.000000',1.0,NULL,'RequestToLeave',13,NULL,'2025-07-01 01:00:07.639766','2025-07-02 03:15:03.343739',' Request to Leave',NULL,NULL,NULL,85),(88,NULL,13,'2025-07-14 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:04:27.946263','2025-07-01 01:04:27.945843',NULL,NULL,NULL,'community center 1',NULL),(89,NULL,13,'2025-07-15 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:04:49.705767','2025-07-01 01:04:49.705762',NULL,NULL,NULL,'community center 1',NULL),(90,NULL,13,'2025-07-17 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:05:41.514769','2025-07-01 01:05:41.514722',NULL,NULL,NULL,'community center 1',NULL),(91,NULL,13,'2025-07-18 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:06:10.643463','2025-07-01 01:06:10.643458',NULL,NULL,NULL,'community center 1',NULL),(92,NULL,13,'2025-07-21 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:06:35.373814','2025-07-01 01:06:35.373810',NULL,NULL,NULL,'community center 1',NULL),(93,NULL,13,'2025-07-22 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:07:19.321114','2025-07-01 01:07:19.321102',NULL,NULL,NULL,'community center 1',NULL),(94,8,13,'2025-07-14 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.621295','2025-07-02 03:15:03.357640',NULL,NULL,NULL,NULL,88),(95,8,13,'2025-07-15 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.638496','2025-07-02 03:15:03.371798',NULL,NULL,NULL,NULL,89),(96,8,13,'2025-07-17 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.656791','2025-07-02 03:15:03.384739',NULL,NULL,NULL,NULL,90),(97,8,13,'2025-07-18 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.672793','2025-07-02 03:15:03.397690',NULL,NULL,NULL,NULL,91),(98,8,13,'2025-07-21 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.688650','2025-07-02 03:15:03.413206',NULL,NULL,NULL,NULL,92),(99,8,13,'2025-07-22 14:00:00.000000',1.0,NULL,'Scheduled',13,NULL,'2025-07-01 01:08:10.704684','2025-07-02 03:15:03.428291',NULL,NULL,NULL,NULL,93),(100,NULL,13,'2025-07-30 14:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-01 01:10:03.697129','2025-07-01 01:10:03.697124',NULL,NULL,NULL,'community center 1',NULL),(101,8,13,'2025-07-30 14:00:00.000000',1.0,NULL,'Deleted',13,NULL,'2025-07-01 22:51:43.002890','2025-07-02 02:59:29.035090',NULL,NULL,NULL,NULL,100),(102,8,13,'2025-07-30 14:00:00.000000',1.0,NULL,'Deleted',13,NULL,'2025-07-01 23:14:16.864729','2025-07-02 03:15:03.443275',NULL,NULL,NULL,NULL,100),(103,8,13,'2025-07-30 14:00:00.000000',1.0,NULL,'Deleted',13,NULL,'2025-07-01 23:15:11.830545','2025-07-01 23:15:11.830543',' Need to reschedule',NULL,NULL,NULL,100),(104,8,17,'2025-07-04 09:00:00.000000',2.0,NULL,'Deleted',22,NULL,'2025-07-03 18:20:07.698224','2025-07-03 18:20:07.697999',' Please reschedule to July 6',NULL,' OK',NULL,43),(105,8,17,'2025-07-06 09:00:00.000000',2.0,2.0,'Completed',22,NULL,'2025-07-03 18:22:26.165923','2025-07-06 12:28:41.347865',NULL,NULL,NULL,NULL,43),(106,8,17,'2025-07-05 10:59:00.000000',1.0,1.0,'Completed',22,NULL,'2025-07-05 08:25:34.822260','2025-07-06 12:28:32.368252',NULL,NULL,NULL,NULL,43),(107,8,17,'2025-07-09 15:00:00.000000',2.0,NULL,'Deleted',22,NULL,'2025-07-06 12:19:27.728126','2025-07-06 12:19:27.727925',NULL,NULL,NULL,NULL,43),(108,8,17,'2025-07-10 19:00:00.000000',1.0,NULL,'Deleted',22,NULL,'2025-07-06 15:07:29.875738','2025-07-06 15:07:29.875499',' Request to Reschedule',NULL,NULL,NULL,43),(109,NULL,13,'2025-08-01 19:48:00.000000',2.0,NULL,'Open',13,NULL,'2025-07-07 19:49:00.493436','2025-07-07 19:49:00.493073',NULL,NULL,NULL,'community center 1',NULL),(110,8,13,'2025-08-01 19:48:00.000000',2.0,NULL,'Scheduled',13,NULL,'2025-07-07 19:50:13.899620','2025-07-07 19:50:13.899489',NULL,NULL,NULL,NULL,109),(113,8,17,'2025-07-09 06:00:00.000000',1.0,1.0,'Completed',22,NULL,'2025-07-09 07:26:00.013666','2025-07-10 13:56:46.044928',NULL,NULL,NULL,NULL,43),(114,NULL,18,'2025-06-02 17:17:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:18:21.676173','2025-07-09 17:18:21.675984',NULL,NULL,NULL,'community center 2',NULL),(115,NULL,18,'2025-06-09 17:18:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:19:04.281673','2025-07-09 17:19:04.281669',NULL,NULL,NULL,'community center 2',NULL),(116,NULL,18,'2025-07-10 17:20:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:20:49.884530','2025-07-09 17:20:49.884528',NULL,NULL,NULL,'community center 2',NULL),(117,NULL,18,'2025-07-11 17:20:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-09 17:21:07.179871','2025-07-09 17:21:07.179869',NULL,NULL,NULL,'community center 2',NULL),(118,8,18,NULL,0.0,NULL,'Registered',13,NULL,'2025-07-09 17:21:39.313093','2025-07-09 17:21:39.313049',NULL,NULL,NULL,NULL,NULL),(119,8,18,'2025-06-02 17:17:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:22:25.054730','2025-07-09 17:22:25.054706',NULL,NULL,NULL,NULL,114),(120,8,18,'2025-06-09 17:18:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:22:25.063093','2025-07-09 17:22:25.063092',NULL,NULL,NULL,NULL,115),(121,8,18,'2025-07-10 17:20:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-09 17:22:25.069044','2025-07-09 17:22:25.069043',NULL,NULL,NULL,NULL,116),(122,8,18,'2025-07-11 17:20:00.000000',1.0,NULL,'OnLeave',13,NULL,'2025-07-09 17:22:25.074783','2025-07-09 17:22:25.074781',NULL,NULL,NULL,NULL,117),(123,8,17,'2025-07-11 07:08:00.000000',1.0,NULL,'Scheduled',22,NULL,'2025-07-10 07:12:04.495841','2025-07-10 07:12:04.495116',NULL,NULL,NULL,NULL,43),(126,NULL,18,'2025-07-02 08:28:00.000000',2.0,NULL,'Completed',13,NULL,'2025-07-10 08:28:40.894034','2025-07-10 08:28:40.894027',NULL,NULL,NULL,'community center 2',NULL),(127,NULL,18,'2025-07-11 09:06:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-10 09:18:18.295345','2025-07-10 09:18:18.294824',NULL,NULL,NULL,'community center 2',NULL),(128,NULL,18,'2025-07-10 12:20:00.000000',1.0,NULL,'Completed',13,NULL,'2025-07-10 09:19:05.410067','2025-07-10 09:19:05.410062',NULL,NULL,NULL,'community center 2',NULL),(129,NULL,19,'2025-08-01 10:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-10 10:44:44.593545','2025-07-10 10:44:44.592900',NULL,NULL,NULL,'community center 3',NULL),(130,NULL,19,'2025-08-08 10:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-10 10:45:14.072565','2025-07-10 10:45:14.072560',NULL,NULL,NULL,'community center 3',NULL),(131,NULL,19,'2025-08-15 10:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-10 10:45:33.413600','2025-07-10 10:45:33.413595',NULL,NULL,NULL,'community center 3',NULL),(132,NULL,19,'2025-08-22 10:00:00.000000',1.0,NULL,'Open',13,NULL,'2025-07-10 10:45:58.179298','2025-07-10 10:45:58.179293',NULL,NULL,NULL,'community center 3',NULL),(134,8,19,'2025-08-01 10:00:00.000000',1.0,NULL,'Registered',13,NULL,'2025-07-10 10:47:59.227208','2025-07-10 10:47:59.226900',NULL,NULL,NULL,NULL,129),(135,8,19,'2025-08-08 10:00:00.000000',1.0,NULL,'Registered',13,NULL,'2025-07-10 10:47:59.247921','2025-07-10 10:47:59.247916',NULL,NULL,NULL,NULL,130),(136,8,19,'2025-08-15 10:00:00.000000',1.0,NULL,'Registered',13,NULL,'2025-07-10 10:47:59.270264','2025-07-10 10:47:59.270258',NULL,NULL,NULL,NULL,131),(137,8,19,'2025-08-22 10:00:00.000000',1.0,NULL,'Registered',13,NULL,'2025-07-10 10:47:59.286933','2025-07-10 10:47:59.286928',NULL,NULL,NULL,NULL,132),(138,8,17,'2025-07-12 17:05:00.000000',1.0,NULL,'Deleted',22,NULL,'2025-07-10 14:02:56.022742','2025-07-10 14:02:56.022573',' I am not feeling well.',NULL,NULL,NULL,43),(139,8,17,'2025-07-24 17:40:00.000000',2.0,NULL,'Scheduled',22,NULL,'2025-07-10 17:40:23.299922','2025-07-10 17:40:23.299077',NULL,NULL,NULL,'community center 3',43);
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
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CourseType` varchar(45) DEFAULT NULL,
  `MaxCapacity` int DEFAULT NULL,
  `SessionCount` int DEFAULT NULL,
  `HourlyCost` decimal(10,2) NOT NULL,
  `HourlyCost2` decimal(10,2) NOT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `courses`
--

LOCK TABLES `courses` WRITE;
/*!40000 ALTER TABLE `courses` DISABLE KEYS */;
INSERT INTO `courses` VALUES (5,'Private swimming with Coach2','Private swimming with Coach2\r\nTest','Private',NULL,12,30.00,20.00,1,2,2,13,13,'2025-03-26 23:21:40.553110','2025-06-06 15:30:23.725027'),(6,'Group skating with Coach2','Group skating with Coach2','Private',NULL,NULL,30.00,20.00,0,2,3,13,13,'2025-03-26 23:22:33.924552','2025-06-06 14:55:40.373592'),(7,'Private swimming with Coach3','Private swimming with Coach3','Private',NULL,NULL,30.00,20.00,1,4,2,13,13,'2025-03-29 00:52:05.725188','2025-06-06 15:30:40.630009'),(11,'Table Tennis course by coach_2025','dasfsa','Private',NULL,NULL,50.00,40.00,1,8,4,13,13,'2025-04-30 10:01:50.572392','2025-06-06 14:56:07.030291'),(13,'group swimming class','group swimming class description','Group',20,12,60.00,50.00,1,NULL,2,13,13,'2025-06-02 17:06:55.118182','2025-06-10 23:08:39.305503'),(15,'Private Basketball Course with coach6','Private Basketball Course with coach6','Private',NULL,12,120.00,100.00,1,7,6,13,13,'2025-06-02 17:26:20.749807','2025-06-06 15:50:39.361269'),(16,'swimming course with coach_2025','swimming course with coach_2025','Private',NULL,12,50.00,40.00,1,8,2,13,13,'2025-06-06 14:32:47.272404','2025-06-06 14:53:55.692321'),(17,'private skiing course with coach4','private skiing course with coach4 description','Private',NULL,10,120.00,100.00,1,5,5,13,NULL,'2025-06-09 15:53:27.629569',NULL),(18,'Group Skating Course','Group Skating Course','Group',NULL,4,120.00,100.00,1,NULL,3,13,NULL,'2025-07-09 17:17:14.269114',NULL),(19,'Basketball group in August 2025','Basketball group in August 2025','Group',NULL,4,120.00,100.00,1,NULL,6,13,NULL,'2025-07-10 10:44:01.210455',NULL);
/*!40000 ALTER TABLE `courses` ENABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parent_child`
--

LOCK TABLES `parent_child` WRITE;
/*!40000 ALTER TABLE `parent_child` DISABLE KEYS */;
INSERT INTO `parent_child` VALUES (1,3,1,1,1,'2025-03-21 13:23:30.056754','2025-03-21 13:23:30.054364','Mother'),(2,4,2,13,13,'2025-03-21 13:34:28.160262','2025-03-21 13:34:28.160015','Mother'),(3,5,2,13,13,'2025-03-21 13:34:58.739003','2025-03-21 13:34:58.739002','Father'),(7,9,8,13,13,'2025-04-07 20:15:42.285022','2025-04-07 20:15:42.284769','Mother'),(8,10,8,13,13,'2025-04-08 13:48:25.435626','2025-04-08 13:48:25.435545','Father'),(9,11,4,13,13,'2025-04-11 13:16:11.315024','2025-04-11 13:16:11.314931','Mother');
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
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parents`
--

LOCK TABLES `parents` WRITE;
/*!40000 ALTER TABLE `parents` DISABLE KEYS */;
INSERT INTO `parents` VALUES (3,'LL',NULL,NULL,NULL,13,NULL,'2025-03-21 13:22:07.261611','2025-03-21 13:22:07.252101'),(4,'LL',NULL,NULL,NULL,13,NULL,'2025-03-21 13:33:50.551450','2025-03-21 13:33:50.541840'),(5,'YY',NULL,NULL,NULL,13,NULL,'2025-03-21 13:34:58.727327','2025-03-21 13:34:58.727324'),(9,'Anne Chen','6478631234','anne@hotmail.com','anna_wechat',13,NULL,'2025-04-07 20:15:42.144410','2025-04-07 20:15:42.143907'),(10,'Peter Wu','6478632312','peter@hotmail.com','peter_wechat',13,NULL,'2025-04-08 13:48:25.343277','2025-04-08 13:48:25.343101'),(11,'Mom1','9051111111','mom1@hotmail.com','mom1_wechat',13,NULL,'2025-04-11 13:16:11.289331','2025-04-11 13:16:11.289120');
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_package`
--

LOCK TABLES `payment_package` WRITE;
/*!40000 ALTER TABLE `payment_package` DISABLE KEYS */;
INSERT INTO `payment_package` VALUES (1,'Package #1','50 Token',6350.00,0,0,NULL,'2025-04-06 18:44:55.760955','2025-04-06 18:44:55.760962'),(2,'Package #2','100 Token',12550.00,1,0,NULL,'2025-03-21 13:58:55.899344','2025-03-21 13:58:55.899392'),(3,'Package #3','150 token',18500.00,1,0,NULL,'2025-03-21 13:59:39.462588','2025-03-21 13:59:39.462591'),(4,'Package #4','200 Token',30000.00,1,0,NULL,'2025-04-07 09:57:21.973018','2025-04-07 09:57:21.973020');
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
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (3,3,2,12550.00,'/receipts/0337932a-e510-4af2-bbdc-aac3780686e3_1.webp','2025-03-20 00:00:00.000000',NULL,13,NULL,1,'0001-01-01 00:00:00.000000',NULL),(4,11,2,12550.00,'/receipts/1ad40d58-b900-44d4-982b-6e8cfc9ab2fb_2.jpg','2025-04-09 00:00:00.000000',NULL,13,NULL,4,'0001-01-01 00:00:00.000000',NULL),(5,9,2,12550.00,'/receipts/e33b9e93-e7cf-41a2-96e7-8b915dedcc8c_istockphoto-1303107115-1024x1024.jpg','2025-04-23 00:00:00.000000',NULL,13,NULL,8,'0001-01-01 00:00:00.000000',NULL);
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
INSERT INTO `roles` VALUES (1,'Admin','ADMIN',NULL),(2,'Staff','STAFF',NULL),(3,'Coach','COACH',NULL),(4,'Parent','PARENT',NULL),(5,'Child','CHILD',NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `specialties`
--

LOCK TABLES `specialties` WRITE;
/*!40000 ALTER TABLE `specialties` DISABLE KEYS */;
INSERT INTO `specialties` VALUES (2,'Swimming','Swimming',13,NULL,'2025-03-18 19:15:18','2025-03-18 19:15:18'),(3,'Skating','Skating',13,NULL,'2025-03-18 19:18:50','2025-03-18 19:18:50'),(4,'Table tennis','Table tennis',13,NULL,'2025-03-18 19:20:51','2025-03-18 19:20:51'),(5,'Skiing','Skiing',13,NULL,'2025-03-18 19:22:01','2025-03-18 19:22:01'),(6,'Basketball','Basketball',13,NULL,NULL,NULL),(7,'Diving','Diving',13,NULL,NULL,NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
INSERT INTO `staff` VALUES (4,13,'staff1','6473333333','staff1_wechat'),(5,14,'staff2','6473333333','staff2_wechat');
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
INSERT INTO `userroles` VALUES (7,1),(8,1),(9,1),(28,1),(32,1),(33,1),(13,2),(14,2),(19,3),(21,3),(22,3),(29,3),(30,3),(34,3),(24,5),(25,5),(26,5),(31,5);
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
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (7,'Admin',NULL,6,'2025-03-16 06:15:22.085427','2025-03-16 14:04:07.943095','test@hotmail.com','TEST@HOTMAIL.COM','test@hotmail.com','TEST@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEBi3cIWewOh4vB6Wjp4ILYAbmerfcU/7jWV+xYfK8+XdawAfx2c0AqOd19djGKvT1A==','L6YR5TN2WEPSTVLRH5UQFVZACQAT4ZBS','a2429539-e9d2-4add-afa3-d35431d7e7be',NULL,0,0,NULL,1,0),(8,'Admin',6,NULL,'2025-03-16 10:07:22.861836','2025-03-16 10:07:22.861847','new@hotmail.com','NEW@HOTMAIL.COM','new@hotmail.com','NEW@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEBk7bWjxO+5nhRp1Fn5UMGnzt3QFMAzGdj/LslmXLP8Y1ABoac6H26Az7/iXEVYxzQ==','KAIKFEQUKFINGVOUMMV3WOXG77DNJANU','ef79559c-6bab-42ea-a16f-3d232f3b7902',NULL,0,0,NULL,1,0),(9,'Admin',6,NULL,'2025-03-16 10:10:34.016191','2025-03-16 10:10:34.016198','new2@hotmail.com','NEW2@HOTMAIL.COM','new2@hotmail.com','NEW2@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEJTlX4PUb06NxfDiiYBAdWkIIgOlpr7Sz/ikM1uTkvASwayFGMQd9znpHjwmBGAtGw==','KYIHRTUQSLZEE2KFKH2LBEVTB6ZOQSJP','ab4ffd28-284f-465b-84ef-225a24393441',NULL,0,0,NULL,1,0),(11,'Staff',NULL,NULL,'2025-03-17 09:22:11.560931','2025-03-17 05:22:11.541638',NULL,NULL,'staff2@hotmail.com',NULL,0,'',NULL,'f037a3f6-c1e3-4ea3-a7ab-21c20008beff',NULL,0,0,NULL,0,0),(13,'Staff',6,NULL,'2025-03-18 10:09:00.478749','2025-03-18 10:09:00.478761','staff1@hotmail.com','STAFF1@HOTMAIL.COM','staff1@hotmail.com','STAFF1@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEBYs8iuVnqSFbnPr4k3I+aSua8XbVdCvqtnj6cwr8hcTo2lY2PlVAs04hHZ/n/j18g==','CQWG2XW7T45OACNP3U7G2JSCSPTS3LP7','da4e0bfa-89b9-4f87-9efe-c9bb922d757d',NULL,0,0,NULL,1,0),(14,'Staff',6,NULL,'2025-03-18 10:13:10.440172','2025-03-18 10:13:10.440182','staff2@hotmail.com','STAFF2@HOTMAIL.COM','staff2@hotmail.com','STAFF2@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAELKdr2HM+BUncQA7MxRmDkx6hKul4Np5evCFQerwycrAw6di9NXjOnWXuwovxsscBA==','ITSZJKAGIGQHNVOEUHXTMHSVWIX4BHVP','71d388eb-de36-44f5-ba78-04bb07f0c422',NULL,0,0,NULL,1,0),(16,'Child',NULL,NULL,'2025-03-20 20:25:24.194857','2025-05-31 07:01:41.928059',NULL,NULL,'jj@hotmail.com',NULL,0,'nsns123!',NULL,'e50f7924-a717-4f1a-bbd5-a80ab8d4f199',NULL,0,0,NULL,0,0),(17,'Child',13,NULL,'2025-03-21 13:19:23.870460','2025-03-21 13:19:23.870240',NULL,NULL,'cc@hotmail.com',NULL,0,'nsns123!',NULL,'5d711cfb-3b79-4272-8a5c-27cfa367a6d8',NULL,0,0,NULL,0,0),(19,'Coach',13,13,'2025-03-25 20:37:07.318565','2025-06-10 13:54:00.298291','coach2@hotmail.com','COACH2@HOTMAIL.COM','coach2@hotmail.com','COACH2@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEEHmPSK/ScBPxUzDHL3//qAhniMQPPv4X733e7nbYKFf7miWsYpkmJApYksYhEq1mw==','A35NY4TX3CW7HVAZNJCPTZDHAH6EW6UT','43506747-f848-4e9c-a282-00e6ced511ae',NULL,0,0,NULL,1,0),(21,'Coach',13,NULL,'2025-03-29 00:51:05.782870','2025-03-29 00:51:05.782873','coach3@hotmail.com','COACH3@HOTMAIL.COM','coach3@hotmail.com','COACH3@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEAZhwu7brq+wN74mxRCX+zkBE/CV+XbBtrLeFFi+j39GVdzv+ftguYYxstHi3v+9bQ==','KPMQIHIQEUSIPWTAEMR346C3PDH72ZU2','c8ec6741-bc45-4830-8142-fca414899447',NULL,0,0,NULL,1,0),(22,'Coach',13,13,'2025-03-29 13:44:46.276192','2025-04-07 19:27:38.802564','coach4@hotmail.com','COACH4@HOTMAIL.COM','coach4@hotmail.com','COACH4@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEPy6xx8Rk3VterhlwERaBBVUMzJnWo8G9u6FxG0txEl00pUej63wHlVIv+kowk1Yfg==','AYFFPBEDXED5TRHHLQTGEEWRLXSP7HSE','46f4fb6f-42e6-470d-8b84-34a1d9a573d0',NULL,0,0,NULL,1,0),(24,'Child',13,NULL,'2025-03-30 17:02:57.086633','2025-03-30 17:02:57.086637','child1@hotmail.com','CHILD1@HOTMAIL.COM','child1@hotmail.com','CHILD1@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEDnwCcaDU+LNTGvosUwYYaav2GhBTwAExxhgsWEybhimFsMmC2glcduN3oYTHtcIrQ==','GRAYGRW2D66OFJONTEAUUHRZPD63NANM','b9fe1ad9-c49b-4d80-9b50-9edaf0520d1a',NULL,0,0,NULL,1,0),(25,'Child',13,NULL,'2025-04-02 22:24:28.519798','2025-04-02 22:24:28.519804','child2@hotmail.com','CHILD2@HOTMAIL.COM','child2@hotmail.com','CHILD2@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEFUcx9jRdf6O99BuP1+Btk6SW1rgZdDQafEHCofgJkPREL+2NF2N7ErjgtvO+yGsng==','SSE4O5VCZOQ7PBNVDPODVUVG4W6GQ2PD','f987c911-c160-4342-a6d4-02f33029a966',NULL,0,0,NULL,1,0),(26,'Child',13,NULL,'2025-04-02 22:25:06.124909','2025-04-02 22:25:06.124911','child3@hotmail.com','CHILD3@HOTMAIL.COM','child3@hotmail.com','CHILD3@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEHBlVENMWk5FyHFDdzbBEu58hu0dM2Z2qQ9z8iWW6zTm6My9eVYLHFDuKTvIaYXkcw==','U4Q5AM5RPYH2EZPU2JLG4ODHCYBYPCYM','2d21862b-9b6e-4b61-b02c-20266dae18d4',NULL,0,0,NULL,1,0),(28,'Admin',NULL,NULL,'2025-04-05 20:31:07.891123','2025-04-05 20:31:07.891152','admin','ADMIN','admin@nsns.ca','ADMIN@NSNS.CA',1,'AQAAAAIAAYagAAAAEAPsblMGwSqyOQoIZPCycDLUrJE9s4lBDsY/qDhCh05BRrBjJ8WfGJUbF90uzQI72g==','3VJ3YEV7M3YEJMRQNL44UAWYWPNRG26A','8774160a-178d-4453-825e-3b64bc937a13',NULL,0,0,NULL,1,0),(29,'Coach',14,NULL,'2025-04-05 20:32:03.912363','2025-04-05 20:32:03.912374','coach5@hotmail.com','COACH5@HOTMAIL.COM','coach5@hotmail.com','COACH5@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEHnM1z2ifif4KUaWP3UnOWXFd9YIZ81p7d/bHypwE1AbPldgaI+eYpcjbfTAD/nj4Q==','P7CWCNXBYPI6KMWWJ2XESBTJOHSRQ2WH','29cec5fe-8605-40c4-8ae8-06615bcc08a1',NULL,0,0,NULL,1,0),(30,'Coach',13,13,'2025-04-07 09:43:59.903820','2025-04-07 19:19:13.134099','coach6@hotmail.com','COACH6@HOTMAIL.COM','coach6@hotmail.com','COACH6@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEMEaeku58kQbqJKrhqSMLVJ0cE+hfx3YipZL+xNuxe1uiJn6DkL1Vr6I2VHVs9V2lg==','5DN245JVUTH5JD5YWJZQDTIFLQBGGYVK','6dfe5224-dda7-4dbc-9c39-5303d6bae15c',NULL,0,0,NULL,1,0),(31,'Child',13,NULL,'2025-04-07 09:47:31.328255','2025-04-07 09:47:31.328256','child4@hotmail.com','CHILD4@HOTMAIL.COM','child4@hotmail.com','CHILD4@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEIYDQFst2Ysw1fw6Sd0eMpiCr+siD3eJuKj/eh5TLyddvwCmT9LyT3+Gm/yktn7bMw==','IDDWB5VTUSHOQ6K2BBZWEWPF4CPM5QXJ','36aa89ce-191a-4681-abd2-9d9a41bc1793',NULL,0,0,NULL,1,0),(32,'Admin',7,NULL,'2025-04-29 23:17:37.538594','2025-04-29 23:17:37.538604','admin1@hotmail.com','ADMIN1@HOTMAIL.COM','admin1@hotmail.com','ADMIN1@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAECjfjGLS/Bx/eUouN3dnTS1sbYcCL5GXTzUkH7rPaGMqPx3QqbwI6Tm4ZWllf/Rf9w==','5SJHEY2BFZAY2Y2RXI2WG4E7D3W6OK3U','b93db670-d7e5-4e6a-9bee-c3d31dea5349',NULL,0,0,NULL,1,0),(33,'Admin',7,NULL,'2025-04-30 05:33:25.044930','2025-04-30 05:33:25.044935','admin2@hotmail.com','ADMIN2@HOTMAIL.COM','admin2@hotmail.com','ADMIN2@HOTMAIL.COM',0,'AQAAAAIAAYagAAAAEG712i1f0mhWa341RuyrVH5yyMztlHGO1rxzKr2O6Mv1lJsRfa8pX7JDpePF1AY0MA==','CKEMMB3S7N2FEHBNYK33YECRXOOD6FDE','2916aa36-99a7-444d-b26e-02bf08fbb147',NULL,0,0,NULL,1,0),(34,'Coach',13,13,'2025-04-30 09:19:08.982964','2025-04-30 09:20:51.763251','coach_2025@gmail.com','COACH_2025@GMAIL.COM','coach_2025@gmail.com','COACH_2025@GMAIL.COM',0,'AQAAAAIAAYagAAAAEGXTSIQ39IJhP5yH7Kfe6rG0NgLX862wQeoLvf5fO81y0LvtrsxsWilzIxFnl/mg4A==','QSD2MZLX5NE53RA6YWSAKSGMCXPHML3J','d6351d59-7d18-4cbd-a134-474df0dc02ef',NULL,0,0,NULL,1,0);
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

-- Dump completed on 2025-07-10 22:53:11
