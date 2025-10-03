-- ============================================
-- Lab: Practicing Data Manipulation in SQL
-- Database: SampleDB
-- ============================================

-- Step 1. Prepare for the Application
-- --------------------------------------------
-- Create new database
CREATE DATABASE IF NOT EXISTS SampleDB;

-- Use the new database
USE SampleDB;

-- Drop table if exists (clean state)
DROP TABLE IF EXISTS Users;

-- Create Users table
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    Age INT
);

-- Insert sample data
INSERT INTO Users (FirstName, LastName, Email, Age) VALUES
('Aisha', 'Khan', 'aisha.khan@example.com', 29),
('Carlos', 'Garcia', 'carlos.garcia@example.com', 35),
('Mei', 'Chen', 'mei.chen@example.com', 24);

-- Check initial state
SELECT * FROM Users;

-- ============================================
-- Step 2. INSERT Operation
-- --------------------------------------------
-- Add a new user Arjun Patel
INSERT INTO Users (FirstName, LastName, Email, Age)
VALUES ('Arjun', 'Patel', 'arjun.patel@example.com', 41);

-- Verify insert
SELECT * FROM Users;

-- ============================================
-- Step 3. UPDATE Operation
-- --------------------------------------------
-- Update Mei's age to 26
UPDATE Users
SET Age = 26
WHERE FirstName = 'Mei';

-- Verify update
SELECT * FROM Users WHERE FirstName = 'Mei';

-- ============================================
-- Step 4. DELETE Operation
-- --------------------------------------------
-- Delete Carlos Garcia
DELETE FROM Users
WHERE LastName = 'Garcia';

-- Verify delete
SELECT * FROM Users;

-- ============================================
-- Step 5. Practicing Safety Measures
-- --------------------------------------------
-- Danger: Update all users' age to 30 (without WHERE)
-- (In real work, this should be avoided!)
START TRANSACTION;
UPDATE Users
SET Age = 30;

-- Check effect
SELECT * FROM Users;

-- Rollback the change
ROLLBACK;

-- Verify rollback worked
SELECT * FROM Users;

-- Safe update: only for Arjun
UPDATE Users
SET Age = 42
WHERE FirstName = 'Arjun';

-- Verify safe update
SELECT * FROM Users WHERE FirstName = 'Arjun';
