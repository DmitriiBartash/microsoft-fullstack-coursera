# Setting up a Relational Database

**Course 6 — Database Integration and Management** · Module 1 · Lesson 2 · `You Try It!`

> Stand up a working MySQL database from scratch using **Visual Studio Code**. You will
> connect to a MySQL server, **create the `CompanyDB` database** and an `Employees` table,
> add a dedicated user with the right **privileges**, then insert and query data to prove the
> whole setup works end-to-end.

---

## 🎯 Objective

By the end of this activity you will be able to **configure MySQL Server** and use **Visual
Studio Code** to work with a relational database — creating a database and table, securing it
with a scoped user account, and verifying the setup with real SQL.

---

## 🗂️ What you will build

A single MySQL database, `CompanyDB`, with one table and a dedicated application user:

| Object              | Definition                                                              |
| ------------------- | ----------------------------------------------------------------------- |
| Database            | `CompanyDB`                                                              |
| Table               | `Employees` (`EmployeeID`, `FirstName`, `LastName`, `Department`)        |
| User                | `'manager'@'localhost'` with **all privileges** on `CompanyDB.*`         |
| Verification script | `Test.sql` — drops, recreates, seeds, and queries the database           |

**Flow:** `Connect → CREATE DATABASE → CREATE TABLE → CREATE USER + GRANT → INSERT → SELECT`

---

## ✅ Prerequisites

- **MySQL Server** installed and running locally
- **Visual Studio Code** with the **MySQL Shell for VS Code** extension installed
- A MySQL account that can create databases and users (e.g. `root`)
- Connection details to hand: host `localhost`, user `root`, your `root` password

---

## 🛠️ Steps

### Step 1 — Set up the database connection in VS Code

Ensure your connection to the MySQL server is active and ready for running SQL commands.

- Open **Visual Studio Code**.
- Click the **MySQL Shell** icon in the sidebar to access the extension.
- Connect to your MySQL server:
  - Click **New Connection** to open a connection to MySQL.
  - If prompted, enter your MySQL server details (for example, host: `localhost`, user: `root`, password: `password`).
- Confirm the connection by running a simple query. In the editor, create the following SQL statement, then execute it with the **Execute** (lightning bolt) button above the code window:

```sql
SHOW DATABASES;
```

### Step 2 — Configure the database

Create the database and a table for a relational data model.

- In the editor, create a new database:

```sql
CREATE DATABASE CompanyDB;
```

- Set the new database as the default for your session:

```sql
USE CompanyDB;
```

- Create a table named `Employees` with the following structure — `EmployeeID` as an
  auto-incrementing primary key, plus three `VARCHAR(50)` columns:

| Column       | Type          | Constraints                       |
| ------------ | ------------- | --------------------------------- |
| `EmployeeID` | `INT`         | Primary key, `AUTO_INCREMENT`     |
| `FirstName`  | `VARCHAR(50)` |                                   |
| `LastName`   | `VARCHAR(50)` |                                   |
| `Department` | `VARCHAR(50)` |                                   |

```sql
CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50)
);
```

### Step 3 — Configure user accounts and permissions

Secure the database by adding a user account and granting it permissions.

- Create a new user for accessing the database:

```sql
CREATE USER 'manager'@'localhost' IDENTIFIED BY 'StrongPassword123';
```

- Grant this user permissions for the `CompanyDB` database:

```sql
GRANT ALL PRIVILEGES ON CompanyDB.* TO 'manager'@'localhost';
FLUSH PRIVILEGES;
```

- Test the new user by connecting to the database using this account (add a new MySQL Shell
  connection with user `manager` and password `StrongPassword123`).

### Step 4 — Test and verify the setup

Confirm the database and user account are functioning as expected.

- Insert a sample record into the `Employees` table:

```sql
INSERT INTO Employees (FirstName, LastName, Department)
VALUES ('John', 'Doe', 'HR');
```

- Query the table to confirm the record was inserted:

```sql
SELECT * FROM Employees;
```

- Verify that the user `manager` can access and query the database by logging in with that
  account and running the same `SELECT` query.

#### `Test.sql` — full setup and verification script

Run this end-to-end script to recreate the database from a clean state, seed it with several
rows, and verify the result:

```sql
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
```

---

## ▶️ Expected result

`SHOW DATABASES;` lists `CompanyDB`, and the verification queries confirm the setup:
`SELECT DATABASE();` returns `CompanyDB`, `SHOW TABLES;` lists `Employees`, `DESCRIBE
Employees;` shows the four columns with `EmployeeID` as the primary key, and `SELECT * FROM
Employees;` returns the seeded rows (John Doe / HR, Ivan Petrov / IT, Anna Smirnova /
Finance) — each with an auto-generated `EmployeeID`. The `manager` user can run the same
`SELECT` successfully.

---

## ☑️ Definition of done

- [ ] MySQL Shell connection to `localhost` is active and `SHOW DATABASES;` runs successfully
- [ ] `CompanyDB` database created and selected with `USE CompanyDB;`
- [ ] `Employees` table created with `EmployeeID` primary key (`AUTO_INCREMENT`) and three `VARCHAR(50)` columns
- [ ] `'manager'@'localhost'` user created and granted **all privileges** on `CompanyDB.*`
- [ ] A sample record inserted and returned by `SELECT * FROM Employees;`
- [ ] The `manager` user can connect and query `CompanyDB`

---

## 🔑 Key concepts

- **Database vs. session context** — `CREATE DATABASE` defines storage; `USE` sets the active
  database for the session so later statements don't need to qualify the table name.
- **Primary key + `AUTO_INCREMENT`** — `EmployeeID` uniquely identifies each row and MySQL
  fills it automatically, so inserts only supply the meaningful columns.
- **Least-privilege users** — application code connects as a scoped user (`manager`) rather
  than `root`; `GRANT ... ON CompanyDB.*` limits that account to a single database.
- **`GRANT` then `FLUSH PRIVILEGES`** — privilege changes take effect for new grants, and
  flushing reloads the grant tables so the new access is picked up immediately.
- **Verify with a repeatable script** — `Test.sql` uses `DROP ... IF EXISTS` so the whole
  database can be rebuilt deterministically, then `SELECT`/`DESCRIBE` prove it worked.
