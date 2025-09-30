-- Step 1: Create Database
DROP DATABASE IF EXISTS CompanyDB;
CREATE DATABASE CompanyDB;
USE CompanyDB;

-- Step 2: Create Table
DROP TABLE IF EXISTS Employees;
CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50)
);

-- Step 3: Insert Records
INSERT INTO Employees (FirstName, LastName, Department)
VALUES 
('John', 'Doe', 'HR'),
('Ivan', 'Petrov', 'IT'),
('Anna', 'Smirnova', 'Finance');

-- Step 4: Verify
SELECT DATABASE();
SHOW TABLES;
DESCRIBE Employees;
SELECT * FROM Employees;
