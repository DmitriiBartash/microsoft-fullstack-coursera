# Writing SELECT Statements

**Course 6 — Database Integration and Management** · Module 2 · Lesson 1 · `You Try It!`

> Stand up a sample `EmployeeDB` database in MySQL, then practice the three pillars of
> reading data: **`SELECT`** to retrieve columns, **`WHERE`** to filter rows, and
> **`ORDER BY`** to sort results — finishing by combining filtering and sorting into a
> single query.

---

## 🎯 Objective

Retrieve and manipulate data with SQL by writing `SELECT` statements that use `WHERE`
clauses to filter and `ORDER BY` to sort. By the end you will be able to query a table for
exactly the rows and columns you need, in the order you want them.

---

## 🗂️ What you will build

A single MySQL database and table you will query throughout the lab:

| Object          | What it is                                                              |
| --------------- | ---------------------------------------------------------------------- |
| `EmployeeDB`    | The sample database that holds the lab data                            |
| `Employees`     | A table of staff records you will `SELECT`, filter, and sort           |
| `lab_select.sql`| The script that creates the schema, seeds data, and holds every query  |

**Flow:** `CREATE DATABASE → CREATE TABLE → INSERT seed rows → SELECT / WHERE / ORDER BY`

---

## ✅ Prerequisites

- MySQL server installed and running
- Visual Studio Code with a MySQL connection (e.g. the **MySQL** / **SQLTools** extension)
- Ability to run SQL statements against your server

---

## 🛠️ Steps

### Step 1 — Prepare the database setup

Create the sample database, switch to it, then create and populate the `Employees` table.

- Open Visual Studio Code and connect to your MySQL database.
- Create a new database called `EmployeeDB`:

```sql
CREATE DATABASE EmployeeDB;
```

- Switch to the `EmployeeDB` database:

```sql
USE EmployeeDB;
```

- Create a table named `Employees` with the following columns:

| Column            | Type            | Constraints                       |
| ----------------- | --------------- | --------------------------------- |
| `ID`              | `INT`           | Primary Key, Auto Increment       |
| `FirstName`       | `VARCHAR(50)`   |                                   |
| `LastName`        | `VARCHAR(50)`   |                                   |
| `Department`      | `VARCHAR(50)`   |                                   |
| `Salary`          | `DECIMAL(10,2)` |                                   |
| `YearsExperience` | `INT`           |                                   |

- Use the following SQL to create and populate the `Employees` table:

```sql
CREATE TABLE Employees (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10,2),
    YearsExperience INT
);

INSERT INTO Employees (FirstName, LastName, Department, Salary, YearsExperience) VALUES
('John', 'Doe', 'HR', 60000, 10),
('Jane', 'Smith', 'Finance', 70000, 8),
('Michael', 'Brown', 'IT', 50000, 5),
('Emily', 'Davis', 'HR', 45000, 2),
('Chris', 'Wilson', 'Finance', 80000, 15);
```

### Step 2 — Retrieve data using SELECT statements

Learn to retrieve data with basic `SELECT` statements.

- Retrieve **all columns** for **all rows** in the `Employees` table.
- Retrieve only the `FirstName` and `LastName` of employees.
- Retrieve **unique** department names using `DISTINCT`.

```sql
-- 2.1 Retrieve all columns
SELECT * FROM Employees;

-- 2.2 Retrieve only first and last names
SELECT FirstName, LastName FROM Employees;

-- 2.3 Retrieve unique department names
SELECT DISTINCT Department FROM Employees;
```

### Step 3 — Filter data using WHERE clauses

Use the `WHERE` clause to filter rows based on conditions.

- Retrieve all employees from the `HR` department.
- Find employees in the `Finance` department with a salary greater than `60,000`.
- Find employees with more than `5` years of experience **and** a salary less than `70,000`.

```sql
-- 3.1 All employees from HR
SELECT * FROM Employees
WHERE Department = 'HR';

-- 3.2 Finance employees with salary > 60000
SELECT * FROM Employees
WHERE Department = 'Finance' AND Salary > 60000;

-- 3.3 Employees with more than 5 years experience and salary < 70000
SELECT * FROM Employees
WHERE YearsExperience > 5 AND Salary < 70000;
```

### Step 4 — Sort data using ORDER BY

Organize query results with the `ORDER BY` clause.

- Retrieve all employees sorted by their `LastName` in **ascending** order (`ASC`).
- Retrieve employees from the `HR` department, sorted by `Salary` in **descending** order (`DESC`).
- Retrieve the **top 3** highest earners across all departments (using `LIMIT`).

```sql
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
```

### Step 5 — Combine WHERE and ORDER BY

Combine filtering and sorting to write more advanced queries.

- Retrieve employees from the `IT` department with more than `3` years of experience, sorted by `YearsExperience` descending.
- Retrieve employees with a salary **between** `50,000` and `75,000`, sorted by `FirstName` ascending.

```sql
-- 5.1 IT employees with > 3 years experience, sorted by YearsExperience descending
SELECT * FROM Employees
WHERE Department = 'IT' AND YearsExperience > 3
ORDER BY YearsExperience DESC;

-- 5.2 Employees with salary between 50,000 and 75,000, sorted by FirstName ascending
SELECT * FROM Employees
WHERE Salary BETWEEN 50000 AND 75000
ORDER BY FirstName ASC;
```

---

## ▶️ Expected result

Each query returns the expected slice of the five seeded rows. For example:

- **2.3 DISTINCT** returns three rows: `HR`, `Finance`, `IT`.
- **3.2 Finance > 60000** returns only `Jane Smith` (70000) and `Chris Wilson` (80000).
- **4.3 Top 3 earners** returns `Chris Wilson` (80000), `Jane Smith` (70000), `John Doe` (60000), in that order.

---

## ☑️ Definition of done

- [ ] `EmployeeDB` database created and selected with `USE`
- [ ] `Employees` table created with all six columns and the five sample rows inserted
- [ ] `SELECT` queries return all columns, specific columns, and `DISTINCT` departments
- [ ] `WHERE` queries filter by department, salary, and experience (including `AND`)
- [ ] `ORDER BY` queries sort ascending and descending, and `LIMIT` returns the top 3 earners
- [ ] Combined `WHERE` + `ORDER BY` queries (including `BETWEEN`) return the correct rows

---

## 🔑 Key concepts

- **`SELECT` projects columns** — `SELECT *` returns every column, while listing columns
  (`SELECT FirstName, LastName`) returns only what you ask for; `DISTINCT` removes duplicate values.
- **`WHERE` filters rows** — conditions compare columns to values, and `AND` / `OR` combine
  multiple conditions; `BETWEEN x AND y` is shorthand for an inclusive range.
- **`ORDER BY` sorts results** — `ASC` (default) sorts low-to-high, `DESC` high-to-low, and you
  can sort on any column independent of what you select.
- **`LIMIT` caps the rows returned** — pairing `ORDER BY Salary DESC` with `LIMIT 3` is the
  idiomatic MySQL way to get a "top N" result.
- **Clause order matters** — SQL expects `SELECT … FROM … WHERE … ORDER BY … LIMIT`; writing
  them out of order is a syntax error.
