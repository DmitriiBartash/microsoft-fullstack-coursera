# Implementing SQL Functions and Aggregate Functions

**Course 6 — Database Integration and Management** · Module 2 · Lesson 3 · `You Try It!`

> Build a small **MySQL** database (`EmployeeDB`) of employee records, then practice the two
> families of SQL operations on it: **scalar functions** that transform values row-by-row
> (`CONCAT`, `UPPER`, `LOWER`, `LENGTH`, `SUBSTRING`) and **aggregate functions** that
> summarize many rows into one (`COUNT`, `SUM`, `AVG`, `MIN`, `MAX`) — including grouping
> with `GROUP BY`.

---

## 🎯 Objective

Learn to manipulate and analyze relational data in MySQL by applying **SQL scalar functions**
to reshape individual column values and **aggregate functions** to compute summaries across
rows, then combining aggregates with `GROUP BY` to produce per-group and per-year reports.

---

## 🗂️ What you will build

A single MySQL schema with one populated table that you query throughout the lab.

| Object               | Purpose                                                              |
| -------------------- | ------------------------------------------------------------------- |
| Database `EmployeeDB`| Container for the lab data                                          |
| Table `Employees`    | Sample employee records (name, department, salary, hire date)      |
| `lab_functions.sql`  | Consolidated script with every query from Steps 1–5                |

**Flow:** `CREATE DATABASE → CREATE TABLE → INSERT data → scalar functions → aggregates → GROUP BY reports`

---

## ✅ Prerequisites

- A running **MySQL** server (MySQL 8.0+ recommended)
- A SQL client or terminal: `mysql` CLI, MySQL Workbench, or similar
- Basic familiarity with `SELECT` statements

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a small database and populate it with sample data so you have something to query.

- Open your MySQL environment or terminal.
- Create a new database called `EmployeeDB` and switch to it:

```sql
CREATE DATABASE EmployeeDB;
USE EmployeeDB;
```

- Create a table called `Employees` with the following schema:

```sql
CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10, 2),
    HireDate DATE
);
```

- Insert the sample data into the `Employees` table:

```sql
INSERT INTO Employees (FirstName, LastName, Department, Salary, HireDate) VALUES
('Liam', 'Nguyen', 'Engineering', 85000.00, '2020-03-15'),
('Sophia', 'Smith', 'Marketing', 72000.00, '2019-05-22'),
('Raj', 'Patel', 'Sales', 64000.00, '2021-07-01'),
('Aisha', 'Khan', 'HR', 60000.00, '2020-09-12'),
('Carlos', 'Martinez', 'Engineering', 93000.00, '2018-12-01'),
('Chen', 'Zhao', 'Marketing', 77000.00, '2017-11-05'),
('Amara', 'Okafor', 'Sales', 67000.00, '2022-03-18');
```

### Step 2 — Use SQL functions for data manipulation

Write queries that use scalar functions such as `CONCAT`, `UPPER`, `LOWER`, `LENGTH`, and
`SUBSTRING` to transform column values one row at a time.

- Concatenate the first and last names into a single column called `FullName`:

```sql
SELECT CONCAT(FirstName, ' ', LastName) AS FullName
FROM Employees;
```

- Convert the `Department` column to uppercase:

```sql
SELECT UPPER(Department) AS UppercaseDepartment
FROM Employees;
```

- Convert the `LastName` column to lowercase:

```sql
SELECT LOWER(LastName) AS LowercaseLastName
FROM Employees;
```

- Calculate the length of each employee's `FirstName`:

```sql
SELECT FirstName, LENGTH(FirstName) AS NameLength
FROM Employees;
```

- Extract the first three characters of the `LastName` column:

```sql
SELECT LastName, SUBSTRING(LastName, 1, 3) AS FirstThreeLetters
FROM Employees;
```

### Step 3 — Use aggregate functions

Write queries that use aggregate functions such as `COUNT`, `SUM`, `AVG`, `MIN`, and `MAX`
to analyze the data across many rows.

- Count the total number of employees in the company:

```sql
SELECT COUNT(*) AS TotalEmployees FROM Employees;
```

- Calculate the total salary expenditure for all employees:

```sql
SELECT SUM(Salary) AS TotalSalaryExpenditure FROM Employees;
```

- Find the average salary of employees in the Engineering department:

```sql
SELECT AVG(Salary) AS AvgEngineeringSalary
FROM Employees
WHERE Department = 'Engineering';
```

- Identify the minimum salary in the company:

```sql
SELECT MIN(Salary) AS MinSalary FROM Employees;
```

- Determine the maximum salary in the Sales department:

```sql
SELECT MAX(Salary) AS MaxSalesSalary
FROM Employees
WHERE Department = 'Sales';
```

### Step 4 — Combine aggregate functions with `GROUP BY`

Write queries that group rows by a column and apply an aggregate to each group.

- Group employees by `Department` and calculate the total salary for each department:

```sql
SELECT Department, SUM(Salary) AS TotalDeptSalary
FROM Employees
GROUP BY Department;
```

- Group employees by `Department` and find the average salary in each department:

```sql
SELECT Department, AVG(Salary) AS AvgDeptSalary
FROM Employees
GROUP BY Department;
```

- Use `GROUP BY` to count the number of employees in each department:

```sql
SELECT Department, COUNT(*) AS EmployeeCount
FROM Employees
GROUP BY Department;
```

### Step 5 — Explore advanced functions

Experiment with combinations of scalar and aggregate functions.

- Concatenate `FirstName` and `LastName`, then calculate the length of the resulting full name:

```sql
SELECT CONCAT(FirstName, ' ', LastName) AS FullName,
       LENGTH(CONCAT(FirstName, ' ', LastName)) AS FullNameLength
FROM Employees;
```

- Use `COUNT` and `GROUP BY` to determine how many employees were hired in each year
  (extract the year from `HireDate` with `YEAR`):

```sql
SELECT YEAR(HireDate) AS HireYear, COUNT(*) AS EmployeesHired
FROM Employees
GROUP BY YEAR(HireDate)
ORDER BY HireYear;
```

- Use `SUM` and `GROUP BY` to calculate the total salary expenditure per year of hiring:

```sql
SELECT YEAR(HireDate) AS HireYear, SUM(Salary) AS TotalSalary
FROM Employees
GROUP BY YEAR(HireDate)
ORDER BY HireYear;
```

---

## 📜 Full script — `lab_functions.sql`

You can run every query above as a single consolidated script:

```sql
-- ============================================
-- Lab: Implementing SQL Functions and Aggregate Functions
-- Database: EmployeeDB
-- ============================================

-- Step 1. Prepare for the Application
-- --------------------------------------------
CREATE DATABASE IF NOT EXISTS EmployeeDB;
USE EmployeeDB;

-- Drop table if exists (clean start)
DROP TABLE IF EXISTS Employees;

-- Create Employees table
CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10, 2),
    HireDate DATE
);

-- Insert sample data
INSERT INTO Employees (FirstName, LastName, Department, Salary, HireDate) VALUES
('Liam', 'Nguyen', 'Engineering', 85000.00, '2020-03-15'),
('Sophia', 'Smith', 'Marketing', 72000.00, '2019-05-22'),
('Raj', 'Patel', 'Sales', 64000.00, '2021-07-01'),
('Aisha', 'Khan', 'HR', 60000.00, '2020-09-12'),
('Carlos', 'Martinez', 'Engineering', 93000.00, '2018-12-01'),
('Chen', 'Zhao', 'Marketing', 77000.00, '2017-11-05'),
('Amara', 'Okafor', 'Sales', 67000.00, '2022-03-18');

-- Verify data
SELECT * FROM Employees;

-- ============================================
-- Step 2. Using SQL Functions for Data Manipulation
-- ============================================
-- 2.1 Concatenate first and last names into FullName
SELECT CONCAT(FirstName, ' ', LastName) AS FullName
FROM Employees;

-- 2.2 Convert Department to uppercase
SELECT UPPER(Department) AS UppercaseDepartment
FROM Employees;

-- 2.3 Convert LastName to lowercase
SELECT LOWER(LastName) AS LowercaseLastName
FROM Employees;

-- 2.4 Calculate length of each employee's FirstName
SELECT FirstName, LENGTH(FirstName) AS NameLength
FROM Employees;

-- 2.5 Extract first 3 characters of LastName
SELECT LastName, SUBSTRING(LastName, 1, 3) AS FirstThreeLetters
FROM Employees;

-- ============================================
-- Step 3. Using Aggregate Functions
-- ============================================
-- 3.1 Count total number of employees
SELECT COUNT(*) AS TotalEmployees FROM Employees;

-- 3.2 Total salary expenditure
SELECT SUM(Salary) AS TotalSalaryExpenditure FROM Employees;

-- 3.3 Average salary of Engineering department
SELECT AVG(Salary) AS AvgEngineeringSalary
FROM Employees
WHERE Department = 'Engineering';

-- 3.4 Minimum salary in the company
SELECT MIN(Salary) AS MinSalary FROM Employees;

-- 3.5 Maximum salary in Sales department
SELECT MAX(Salary) AS MaxSalesSalary
FROM Employees
WHERE Department = 'Sales';

-- ============================================
-- Step 4. Combining Aggregate Functions with GROUP BY
-- ============================================
-- 4.1 Total salary per department
SELECT Department, SUM(Salary) AS TotalDeptSalary
FROM Employees
GROUP BY Department;

-- 4.2 Average salary per department
SELECT Department, AVG(Salary) AS AvgDeptSalary
FROM Employees
GROUP BY Department;

-- 4.3 Number of employees per department
SELECT Department, COUNT(*) AS EmployeeCount
FROM Employees
GROUP BY Department;

-- ============================================
-- Step 5. Exploring Advanced Functions
-- ============================================
-- 5.1 Concatenate FirstName + LastName and calculate length of FullName
SELECT CONCAT(FirstName, ' ', LastName) AS FullName,
       LENGTH(CONCAT(FirstName, ' ', LastName)) AS FullNameLength
FROM Employees;

-- 5.2 Count employees hired each year
SELECT YEAR(HireDate) AS HireYear, COUNT(*) AS EmployeesHired
FROM Employees
GROUP BY YEAR(HireDate)
ORDER BY HireYear;

-- 5.3 Total salary expenditure per hire year
SELECT YEAR(HireDate) AS HireYear, SUM(Salary) AS TotalSalary
FROM Employees
GROUP BY YEAR(HireDate)
ORDER BY HireYear;
```

---

## ▶️ Expected result

Each query returns a result set against the seven seeded rows. With this sample data you
should see, for example:

| Query                                  | Result                                                                 |
| -------------------------------------- | ---------------------------------------------------------------------- |
| `COUNT(*)` total employees             | `7`                                                                    |
| `SUM(Salary)` total expenditure        | `518000.00`                                                            |
| `AVG(Salary)` for Engineering          | `89000.00` (average of 85000 and 93000)                                |
| `MIN(Salary)` company-wide             | `60000.00` (Aisha Khan, HR)                                            |
| `MAX(Salary)` in Sales                 | `67000.00` (Amara Okafor)                                              |
| Employees hired per year (`GROUP BY`)  | one row per year: 2017, 2018, 2019, 2020 (×2), 2021, 2022             |

The scalar-function queries return one transformed row per employee (e.g. `FullName`,
uppercase departments, `LENGTH` of each first name), and the `GROUP BY` queries return one
summary row per department or per hire year.

---

## ☑️ Definition of done

- [ ] Database `EmployeeDB` and table `Employees` created
- [ ] All seven sample rows inserted and visible via `SELECT * FROM Employees;`
- [ ] Scalar-function queries run (`CONCAT`, `UPPER`, `LOWER`, `LENGTH`, `SUBSTRING`)
- [ ] Aggregate queries run (`COUNT`, `SUM`, `AVG`, `MIN`, `MAX`) and return the expected totals
- [ ] `GROUP BY` queries produce per-department and per-year summaries
- [ ] Advanced combinations (nested `CONCAT`/`LENGTH`, `YEAR` + `COUNT`/`SUM`) execute correctly

---

## 🔑 Key concepts

- **Scalar vs. aggregate functions** — scalar functions (`CONCAT`, `UPPER`, `LENGTH`, `SUBSTRING`)
  transform *one row at a time* and return a value per row; aggregate functions (`COUNT`, `SUM`,
  `AVG`, `MIN`, `MAX`) collapse *many rows into a single value*.
- **`GROUP BY` partitions the data** — it splits rows into buckets by the grouped column, and the
  aggregate is computed independently for each bucket, turning a company-wide total into a
  per-department or per-year breakdown.
- **Filtering before aggregating** — a `WHERE` clause (e.g. `Department = 'Engineering'`) narrows
  the rows *before* the aggregate runs, so `AVG`/`MAX` apply only to the matching subset.
- **Functions compose** — you can nest one function inside another (`LENGTH(CONCAT(...))`) or wrap a
  column in a function inside `GROUP BY` (`GROUP BY YEAR(HireDate)`) to derive new grouping keys.
- **Aliases make output readable** — `AS FullName`, `AS TotalSalaryExpenditure`, etc. name the
  computed columns so results are self-describing instead of showing the raw expression.
