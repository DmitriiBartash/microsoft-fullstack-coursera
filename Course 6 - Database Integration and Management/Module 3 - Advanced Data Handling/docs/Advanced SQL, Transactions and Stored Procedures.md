# Advanced SQL, Transactions and Stored Procedures

**Course 6 — Database Integration and Management** · Module 3 · `You Try It!`

> Build a MySQL database (`EmployeeDB`), seed it with sample data, then automate
> SQL work with a **stored procedure** (`IncreaseSalary`) and a **scalar function**
> (`CalculateBonus`). Along the way you wrap the update in a **transaction** and add
> **validation + error handling** with `SIGNAL`.

---

## 🎯 Objective

By the end of this activity you will have created and executed **stored procedures**
and **functions** that automate SQL operations. You will set up a database, populate it
with sample data, and manage SQL tasks server-side — reinforcing your ability to work
with advanced SQL techniques, manage **transactions**, and optimize database operations.

---

## 🗂️ What you will build

A single MySQL database, `EmployeeDB`, with one table and two routines:

| Object                       | Type             | Responsibility                                              |
| ---------------------------- | ---------------- | ----------------------------------------------------------- |
| `Employees`                  | Table            | Stores employee records (name, department, salary, hire date) |
| `IncreaseSalary(dept, incr)` | Stored procedure | Raise salaries for one department, inside a transaction     |
| `CalculateBonus(salary)`     | Scalar function  | Return a 10% bonus for a given salary                       |

**Flow:** `CREATE DATABASE → CREATE TABLE → seed rows → CREATE PROCEDURE → CALL → CREATE FUNCTION → SELECT … CalculateBonus()`

---

## ✅ Prerequisites

- **MySQL Server 8.x** installed and running
- **Visual Studio Code** with a MySQL client extension (e.g. *MySQL* / *SQLTools*) connected to your server
- A connection account with privileges to `CREATE DATABASE`, `CREATE ROUTINE`, and run DML

---

## 🛠️ Steps

### Step 1 — Prepare the database and seed data

Open MySQL in Visual Studio Code, then create the `EmployeeDB` database, define the
`Employees` table, and load diverse sample data.

```sql
-- Create and switch to the database
CREATE DATABASE EmployeeDB;
USE EmployeeDB;

-- Clean start (safe to re-run)
DROP TABLE IF EXISTS Employees;

-- Define the Employees table
CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName  VARCHAR(50),
    LastName   VARCHAR(50),
    Department VARCHAR(50),
    Salary     DECIMAL(10, 2),
    HireDate   DATE
);

-- Populate with sample data
INSERT INTO Employees (FirstName, LastName, Department, Salary, HireDate)
VALUES
    ('Aisha', 'Khan',   'Finance',   85000.00, '2019-03-15'),
    ('Luis',  'Garcia', 'IT',        95000.00, '2020-07-22'),
    ('Chloe', 'Nguyen', 'Marketing', 72000.00, '2018-10-05'),
    ('Amara', 'Smith',  'HR',        67000.00, '2021-01-18'),
    ('Ravi',  'Patel',  'Finance',   88000.00, '2017-11-03');
```

Verify the seed by listing every record:

```sql
SELECT * FROM Employees;
```

### Step 2 — Create the `IncreaseSalary` stored procedure

Define a procedure that takes two parameters — `deptName` (department) and
`p_increment` (amount to add) — and raises the salary of everyone in that department.
The update runs inside an explicit **transaction**, and the body validates its inputs
before touching any data.

```sql
DROP PROCEDURE IF EXISTS IncreaseSalary;

DELIMITER $$

CREATE PROCEDURE IncreaseSalary(
    IN deptName VARCHAR(50),
    IN p_increment DECIMAL(10,2)
)
BEGIN
    DECLARE deptCount INT;
    DECLARE errMsg VARCHAR(255);

    -- Validate increment is positive
    IF p_increment <= 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Error: Increment must be greater than zero.';
    END IF;

    -- Check the department exists
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
```

Execute the procedure and verify the affected rows:

```sql
CALL IncreaseSalary('Finance', 2000.00);
SELECT * FROM Employees WHERE Department = 'Finance';
```

> The two `Finance` employees (Aisha and Ravi) should each gain `2000.00`.

### Step 3 — Create the `CalculateBonus` scalar function

Define a function that takes one parameter — `salary` — and returns the annual bonus
as **10% of salary**. It is marked `DETERMINISTIC` (same input always yields the same
output) and validates that the salary is positive.

```sql
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
```

Use the function in a query to show each employee's bonus:

```sql
SELECT FirstName, LastName, Salary, CalculateBonus(Salary) AS Bonus
FROM Employees;
```

### Step 4 — Best practices: validation and error handling

The procedure and function above already follow best practice — they **fail fast** with a
clear message instead of corrupting data:

- **`IncreaseSalary`** rejects a non-positive `increment` and reports when the department
  does not exist, raising a user-defined error via `SIGNAL SQLSTATE '45000'`.
- **`CalculateBonus`** rejects a salary of zero or less the same way.

Try each guard rail — these should all raise an error rather than run:

```sql
CALL IncreaseSalary('Finance', -100);  -- invalid increment
CALL IncreaseSalary('Legal', 2000);    -- nonexistent department
SELECT CalculateBonus(-5000);          -- invalid salary
```

Confirm the final state of the table:

```sql
SELECT * FROM Employees;
```

> The full, runnable script for all four steps is provided as `lab_advanced_sql.sql`.

---

## ▶️ Expected result

- `SELECT * FROM Employees;` returns the **five seeded rows**.
- After `CALL IncreaseSalary('Finance', 2000.00);`, both **Finance** salaries rise by `2000.00`
  (Aisha → `87000.00`, Ravi → `90000.00`).
- The `CalculateBonus` query lists every employee with a **Bonus** column equal to 10% of salary.
- Each invalid call in Step 4 stops with a descriptive error message (e.g. *"Error: Department Legal not found."*) and changes no data.

---

## ☑️ Definition of done

- [ ] `EmployeeDB` database and `Employees` table created and seeded with the five sample rows
- [ ] `IncreaseSalary` procedure created, wraps the `UPDATE` in `START TRANSACTION … COMMIT`, and validates inputs
- [ ] `CALL IncreaseSalary('Finance', 2000.00)` updates both Finance salaries correctly
- [ ] `CalculateBonus` function created as `DETERMINISTIC` and returns 10% of salary
- [ ] The bonus query lists every employee with the correct `Bonus`
- [ ] Each invalid Step 4 call raises an error via `SIGNAL` and leaves the data unchanged

---

## 🔑 Key concepts

- **Stored procedures** encapsulate multi-statement logic on the server, accepting `IN`
  parameters and centralizing rules so applications call one routine instead of repeating SQL.
- **Scalar functions** return a single value and can be used inline in `SELECT`; marking them
  `DETERMINISTIC` tells MySQL the result depends only on the inputs.
- **Transactions** (`START TRANSACTION … COMMIT`) make a set of changes atomic — they all
  apply together, which protects data integrity for the salary update.
- **Validation and error handling** with `SIGNAL SQLSTATE '45000'` let routines *fail fast*
  with a clear `MESSAGE_TEXT` instead of silently producing wrong results.
- **Idempotent scripts** — guarding each object with `DROP … IF EXISTS` (and `CREATE DATABASE IF NOT EXISTS`) lets you re-run the lab cleanly during development.
