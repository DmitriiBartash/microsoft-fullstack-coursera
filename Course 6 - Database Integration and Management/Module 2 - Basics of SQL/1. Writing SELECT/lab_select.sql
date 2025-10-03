-- Step 1. Prepare for the Database Setup
USE DbFullStackCourseraLab;

-- Drop table if it exists (clean state)
DROP TABLE IF EXISTS Employees;

-- Create Employees table
CREATE TABLE Employees (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10,2),
    YearsExperience INT
);

-- Insert sample data
INSERT INTO Employees (FirstName, LastName, Department, Salary, YearsExperience) VALUES
('John', 'Doe', 'HR', 60000, 10),
('Jane', 'Smith', 'Finance', 70000, 8),
('Michael', 'Brown', 'IT', 50000, 5),
('Emily', 'Davis', 'HR', 45000, 2),
('Chris', 'Wilson', 'Finance', 80000, 15);

-- ============================================
-- Step 2: Retrieve Data Using SELECT Statements
-- ============================================

-- 2.1 Retrieve all columns
SELECT * FROM Employees;

-- 2.2 Retrieve only first and last names
SELECT FirstName, LastName FROM Employees;

-- 2.3 Retrieve unique department names
SELECT DISTINCT Department FROM Employees;

-- ============================================
-- Step 3: Filter Data Using WHERE Clauses
-- ============================================

-- 3.1 All employees from HR
SELECT * FROM Employees
WHERE Department = 'HR';

-- 3.2 Finance employees with salary > 60000
SELECT * FROM Employees
WHERE Department = 'Finance' AND Salary > 60000;

-- 3.3 Employees with more than 5 years experience and salary < 70000
SELECT * FROM Employees
WHERE YearsExperience > 5 AND Salary < 70000;

-- ============================================
-- Step 4: Sort Data Using ORDER BY
-- ============================================

-- 4.1 All employees sorted by last name ascending
SELECT * FROM Employees
ORDER BY LastName ASC;

-- 4.2 HR employees sorted by salary descending
SELECT * FROM Employees
WHERE Department = 'HR'
ORDER BY Salary DESC;

-- 4.3 Top 3 employees by salary
SELECT * FROM Employees
ORDER BY Salary DESC
LIMIT 3;

-- ============================================
-- Step 5: Combine WHERE and ORDER BY
-- ============================================

-- 5.1 IT employees with > 3 years experience, sorted by YearsExperience descending
SELECT * FROM Employees
WHERE Department = 'IT' AND YearsExperience > 3
ORDER BY YearsExperience DESC;

-- 5.2 Employees with salary between 50,000 and 75,000, sorted by FirstName ascending
SELECT * FROM Employees
WHERE Salary BETWEEN 50000 AND 75000
ORDER BY FirstName ASC;
