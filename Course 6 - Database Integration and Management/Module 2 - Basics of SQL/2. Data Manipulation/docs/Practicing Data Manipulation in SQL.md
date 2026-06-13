# Practicing Data Manipulation in SQL

**Course 6 — Database Integration and Management** · Module 2 · Lesson 2 · `You Try It!`

> Build a small MySQL practice database and run the three core data-manipulation
> statements — **`INSERT`**, **`UPDATE`**, and **`DELETE`** — against a `Users` table.
> The closing step drives home the most important habit in SQL: always scope writes
> with a `WHERE` clause, and wrap risky changes in a transaction you can `ROLLBACK`.

---

## 🎯 Objective

Practice the core SQL data-manipulation operations against a real MySQL table by adding,
modifying, and removing rows, and learn to perform these changes **safely** using the
`WHERE` clause and transactions so a single statement can never silently rewrite every row.

---

## 🗂️ What you will build

A MySQL database named **`SampleDB`** containing one table, **`Users`**, that you will exercise
with each data-manipulation statement:

| Operation     | Statement | What it does in this lab                                  |
| ------------- | --------- | --------------------------------------------------------- |
| Create rows   | `INSERT`  | Add **Arjun Patel** to the `Users` table                  |
| Modify rows   | `UPDATE`  | Change **Mei**'s age to `26`                              |
| Remove rows   | `DELETE`  | Delete the user whose `LastName` is **`Garcia`**          |
| Stay safe     | `WHERE` + `ROLLBACK` | Undo an unscoped update, then redo it correctly |

**Flow:** `CREATE DATABASE → CREATE TABLE → INSERT → UPDATE → DELETE → START TRANSACTION → ROLLBACK`

---

## ✅ Prerequisites

- MySQL Server installed and running
- Visual Studio Code (with an integrated terminal or a MySQL client extension)
- The `mysql` command-line client available on your `PATH`
- A MySQL user account with privileges to create databases and tables

---

## 🛠️ Steps

### Step 1 — Prepare the application

You will create a small application to manage a sample database using MySQL. The application
lets you perform `INSERT`, `UPDATE`, and `DELETE` operations on a table called `Users`.

1. Open **Visual Studio Code** and connect it to a MySQL database using the terminal.
2. Create a new database called `SampleDB`.
3. Switch to the new database.
4. Create a table named `Users`.
5. Populate `Users` with sample data.

```sql
-- Create the database and select it
CREATE DATABASE SampleDB;
USE SampleDB;

-- Create the Users table
CREATE TABLE Users (
    UserID    INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName  VARCHAR(50),
    Email     VARCHAR(100),
    Age       INT
);

-- Populate with sample data
INSERT INTO Users (FirstName, LastName, Email, Age) VALUES
    ('Aisha',  'Khan',   'aisha.khan@example.com',   29),
    ('Carlos', 'Garcia', 'carlos.garcia@example.com', 35),
    ('Mei',    'Chen',   'mei.chen@example.com',      24);
```

### Step 2 — Implement an `INSERT` operation

Write a query to add a new user to the `Users` table, then confirm it landed.

Add a new user with these details:

| Field       | Value                     |
| ----------- | ------------------------- |
| `FirstName` | `Arjun`                   |
| `LastName`  | `Patel`                   |
| `Email`     | `arjun.patel@example.com` |
| `Age`       | `41`                      |

```sql
INSERT INTO Users (FirstName, LastName, Email, Age)
VALUES ('Arjun', 'Patel', 'arjun.patel@example.com', 41);

-- Ensure the user was added
SELECT * FROM Users;
```

### Step 3 — Implement an `UPDATE` operation

Write a query to update the age of an existing user. Update the age of the user whose
`FirstName` is `Mei` to `26`, then confirm **only** that row changed.

```sql
UPDATE Users
SET Age = 26
WHERE FirstName = 'Mei';

-- Ensure only the targeted record was updated
SELECT * FROM Users WHERE FirstName = 'Mei';
```

### Step 4 — Implement a `DELETE` operation

Write a query to delete a user from the `Users` table. Delete the user with the `LastName`
`Garcia`, then verify the row is gone.

```sql
DELETE FROM Users
WHERE LastName = 'Garcia';

-- Verify the record was deleted
SELECT * FROM Users;
```

### Step 5 — Practice safety measures

Practice safe SQL by using the `WHERE` clause to avoid unintentional changes. Wrap the risky
update in a transaction so it can be undone.

1. Attempt to update the age of **all** users to `30` **without** a `WHERE` clause, and observe
   the behavior — every row changes.
2. Roll back the changes with `ROLLBACK`.
3. Retry the update for a specific user, this time **with** a `WHERE` clause.

```sql
-- Danger: updates EVERY row because there is no WHERE clause.
-- Run it inside a transaction so it can be rolled back.
START TRANSACTION;

UPDATE Users
SET Age = 30;

-- Observe the behavior: all rows now show Age = 30
SELECT * FROM Users;

-- Undo the unscoped change
ROLLBACK;

-- Verify the rollback worked (ages are restored)
SELECT * FROM Users;

-- Safe update: scoped to a single user with WHERE
UPDATE Users
SET Age = 42
WHERE FirstName = 'Arjun';

-- Verify the safe update
SELECT * FROM Users WHERE FirstName = 'Arjun';
```

> *Note:* If `autocommit` is on, an unscoped `UPDATE` is committed immediately and **cannot**
> be rolled back. Opening the change with `START TRANSACTION` is what makes `ROLLBACK` possible.

---

## ▶️ Expected result

- After Step 2, `SELECT * FROM Users` lists **four** users, including the new **Arjun Patel** row.
- After Step 3, **Mei**'s `Age` reads `26` while every other row is unchanged.
- After Step 4, **Carlos Garcia** no longer appears in the table.
- In Step 5, the unscoped `UPDATE` sets every `Age` to `30`; after `ROLLBACK`, the original ages
  return; the final scoped `UPDATE` changes **only Arjun** to `42`.

---

## ☑️ Definition of done

- [ ] `SampleDB` database and `Users` table created and seeded with the three sample users
- [ ] `INSERT` adds **Arjun Patel** and a follow-up `SELECT` confirms four rows
- [ ] `UPDATE` sets **Mei**'s age to `26` and changes no other row
- [ ] `DELETE` removes the user with `LastName` `Garcia`
- [ ] An unscoped `UPDATE` is undone with `ROLLBACK`, then redone safely with a `WHERE` clause
- [ ] Each operation is verified with a `SELECT` query

---

## 🔑 Key concepts

- **`INSERT` / `UPDATE` / `DELETE` are DML** — they change the *data* in a table, unlike DDL
  (`CREATE`, `DROP`) which changes the table's *structure*.
- **`WHERE` scopes a write** — an `UPDATE` or `DELETE` with no `WHERE` clause affects **every row**.
  Treat the `WHERE` clause as a required safety belt, not an optional filter.
- **Transactions make changes reversible** — wrap risky statements in `START TRANSACTION`; `ROLLBACK`
  discards them and `COMMIT` makes them permanent. Without a transaction (autocommit on), there is
  nothing to roll back.
- **Verify with `SELECT`** — after every mutation, read the table back to prove the change did
  exactly what you intended and nothing more.
- **Target by a stable key** — filtering on a unique column (here `UserID`, or a known `FirstName`/
  `LastName`) prevents accidentally matching rows you did not mean to touch.
