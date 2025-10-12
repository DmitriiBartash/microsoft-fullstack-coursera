-- ============================================
-- Lab: Advanced SQL, Transactions, and Stored Procedures
-- Database: EmployeeDB
-- ============================================

CREATE DATABASE IF NOT EXISTS EmployeeDB;
USE EmployeeDB;

-- Clean start
DROP TABLE IF EXISTS Employees;

CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10, 2),
    HireDate DATE
);

INSERT INTO Employees (FirstName, LastName, Department, Salary, HireDate)
VALUES
    ('Aisha', 'Khan', 'Finance', 85000.00, '2019-03-15'),
    ('Luis', 'Garcia', 'IT', 95000.00, '2020-07-22'),
    ('Chloe', 'Nguyen', 'Marketing', 72000.00, '2018-10-05'),
    ('Amara', 'Smith', 'HR', 67000.00, '2021-01-18'),
    ('Ravi', 'Patel', 'Finance', 88000.00, '2017-11-03');

-- Verify seed
SELECT * FROM Employees;

-- ============================================
-- Step 2: Stored Procedure (fixed for MySQL 8.x)
-- ============================================

DROP PROCEDURE IF EXISTS IncreaseSalary;
DELIMITER $$

CREATE PROCEDURE IncreaseSalary(
    IN deptName VARCHAR(50),
    IN p_increment DECIMAL(10,2)
)
BEGIN
    DECLARE deptCount INT;
    DECLARE errMsg VARCHAR(255);

    -- Validate increment
    IF p_increment <= 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Error: Increment must be greater than zero.';
    END IF;

    -- Check department existence
    SELECT COUNT(*) INTO deptCount
    FROM Employees
    WHERE Department = deptName;

    IF deptCount = 0 THEN
        SET errMsg = CONCAT('Error: Department ', deptName, ' not found.');
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = errMsg;
    ELSE
        START TRANSACTION;
            UPDATE Employees
            SET Salary = Salary + p_increment
            WHERE Department = deptName;
        COMMIT;
    END IF;
END$$

DELIMITER ;

-- Test procedure
CALL IncreaseSalary('Finance', 2000.00);
SELECT * FROM Employees WHERE Department = 'Finance';

-- ============================================
-- Step 3: Scalar Function (fixed for MySQL 8.x)
-- ============================================

DROP FUNCTION IF EXISTS CalculateBonus;
DELIMITER $$

CREATE FUNCTION CalculateBonus(salary DECIMAL(10,2))
RETURNS DECIMAL(10,2)
DETERMINISTIC
BEGIN
    DECLARE bonus DECIMAL(10,2);

    IF salary <= 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Error: Salary must be greater than zero.';
    END IF;

    SET bonus = salary * 0.10;
    RETURN bonus;
END$$

DELIMITER ;

-- Test function
SELECT FirstName, LastName, Salary, CalculateBonus(Salary) AS Bonus
FROM Employees;

-- ============================================
-- Step 4: Error handling tests (uncomment to try)
-- ============================================
-- CALL IncreaseSalary('Finance', -100); -- invalid increment
-- CALL IncreaseSalary('Legal', 2000);   -- nonexistent department
-- SELECT CalculateBonus(-5000);         -- invalid salary

-- Final state
SELECT * FROM Employees;
