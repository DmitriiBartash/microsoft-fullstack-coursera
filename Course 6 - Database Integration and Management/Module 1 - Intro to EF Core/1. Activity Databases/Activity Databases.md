# Activity: Databases

**Course 6 — Database Integration and Management** · Module 1 · Lesson 1 · `Activity`

> Practise turning a plain-English domain into a **relational schema**. You will model two
> systems — a **university** and a **library** — identifying the tables, choosing columns,
> wiring up the relationships (One-to-Many and Many-to-Many), and writing the `CREATE TABLE`
> DDL that brings each design to life.

---

## 🎯 Objective

Learn to translate a business problem into a normalized relational database design: pick the
right **tables**, define their **columns** with appropriate types and keys, and resolve
**Many-to-Many** relationships with a junction table — then express the whole design as runnable
SQL DDL with primary and foreign keys.

---

## 📦 What you will produce

Two complete schema designs, each delivered as a table list, a relationship map, and a SQL script:

| Task | Domain | Tables | Key relationship |
| ---- | ------ | ------ | ---------------- |
| Task 1 | University | `Students`, `Professors`, `Courses`, `Enrollments` | Students ↔ Courses (Many-to-Many via `Enrollments`) |
| Task 2 | Library | `Members`, `Books`, `Loans` | Members ↔ Books (Many-to-Many via `Loans`) |

Each design must include: primary keys, foreign keys, sensible column types, `UNIQUE` constraints
where identity demands it, and a junction table that resolves the Many-to-Many link.

---

## ✅ Prerequisites

- Familiarity with relational concepts: **primary key (PK)**, **foreign key (FK)**, and **cardinality**.
- Basic SQL DDL syntax (`CREATE TABLE`, column types, `PRIMARY KEY`, `FOREIGN KEY … REFERENCES`).
- A SQL database or editor to run the scripts (SQL Server, PostgreSQL, MySQL, or SQLite).

---

## 🧭 Walkthrough — Task 1: University Database Schema

### Define the problem

Design a database for a university to manage **students, courses, and professors**.

### Steps to complete the task

1. **Identify the tables:** `Students`, `Courses`, `Professors`.
2. **Decide on the columns** for each table.
3. **Connect the tables with relationships:**
   - Each professor can teach many courses, but a course is taught by one professor — **One-to-Many**.
   - Each student can enroll in many courses, and each course can have many students — **Many-to-Many**.

### Solution

**1. Tables and attributes**

`Students`

| Column | Key | Description |
| ------ | --- | ----------- |
| `StudentID` | PK | Unique student identifier |
| `FirstName` | | First name |
| `LastName` | | Last name |
| `Email` | | Email address |
| `DateOfBirth` | | Date of birth |
| `Major` | | Student's major |

`Professors`

| Column | Key | Description |
| ------ | --- | ----------- |
| `ProfessorID` | PK | Unique professor identifier |
| `FirstName` | | First name |
| `LastName` | | Last name |
| `Email` | | Email address |
| `Department` | | Department name |

`Courses`

| Column | Key | Description |
| ------ | --- | ----------- |
| `CourseID` | PK | Unique course identifier |
| `Title` | | Course title |
| `Credits` | | Number of credits |
| `ProfessorID` | FK | Reference to professor (One-to-Many: one professor → many courses) |

`Enrollments` *(junction table for Many-to-Many)*

| Column | Key | Description |
| ------ | --- | ----------- |
| `EnrollmentID` | PK | Unique enrollment record |
| `StudentID` | FK | Reference to student |
| `CourseID` | FK | Reference to course |
| `Grade` | | Grade (nullable until course completion) |

**2. Relationships**

- **Professors → Courses:** one professor can teach many courses, but each course is taught by one professor (**One-to-Many**).
- **Students ↔ Courses (via `Enrollments`):** a student can enroll in many courses, and each course can have many students (**Many-to-Many**).

**3. ER diagram**

> *(The original includes an ER diagram here.)*

The relationships in shorthand:

```text
Professors 1 ──< Courses 1 ──< Enrollments >── 1 Students
```

**SQL script (DDL)**

```sql
CREATE TABLE Students (
    StudentID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100) UNIQUE,
    DateOfBirth DATE,
    Major VARCHAR(100)
);

CREATE TABLE Professors (
    ProfessorID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100) UNIQUE,
    Department VARCHAR(100)
);

CREATE TABLE Courses (
    CourseID INT PRIMARY KEY,
    Title VARCHAR(100),
    Credits INT,
    ProfessorID INT,
    FOREIGN KEY (ProfessorID) REFERENCES Professors(ProfessorID)
);

CREATE TABLE Enrollments (
    EnrollmentID INT PRIMARY KEY,
    StudentID INT,
    CourseID INT,
    Grade CHAR(2),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
```

---

## 🧩 Your turn — Task 2: Library Management Schema

### Define the problem

Design a database for a library to manage **members, books, and loans**.

### Steps to complete the task

1. **Identify the tables:** `Members`, `Books`, `Loans`.
2. **Decide on the columns** for each table.
3. **Connect the tables with relationships:**
   - A loan connects a member to a book (**One-to-Many** from each side).
   - A book can be borrowed many times by different members (**Many-to-Many**, resolved by `Loans`).

### Solution

**1. Tables and attributes**

`Members`

| Column | Key | Description |
| ------ | --- | ----------- |
| `MemberID` | PK | Unique member identifier |
| `FirstName` | | First name |
| `LastName` | | Last name |
| `Email` | | Email address (unique) |
| `Phone` | | Phone number |
| `DateJoined` | | Date when member joined |

`Books`

| Column | Key | Description |
| ------ | --- | ----------- |
| `BookID` | PK | Unique book identifier |
| `Title` | | Book title |
| `Author` | | Author of the book |
| `ISBN` | | International book number (unique) |
| `PublishedYear` | | Year of publication |
| `CopiesAvailable` | | Number of available copies |

`Loans` *(junction table for Many-to-Many)*

| Column | Key | Description |
| ------ | --- | ----------- |
| `LoanID` | PK | Unique loan identifier |
| `MemberID` | FK | Reference to member |
| `BookID` | FK | Reference to book |
| `LoanDate` | | Date when book was borrowed |
| `DueDate` | | Due date for return |
| `ReturnDate` | | Actual return date (nullable) |

**2. Relationships**

- **Members → Loans:** one member can have many loans (**One-to-Many**).
- **Books → Loans:** one book can appear in many loans (**One-to-Many**).
- **Members ↔ Books (via `Loans`):** a member can borrow many books, and each book can be borrowed by many members (**Many-to-Many**, resolved by `Loans`).

**3. ER diagram**

> *(The original includes an ER diagram here.)*

The relationships in shorthand:

```text
Members 1 ──< Loans >── 1 Books
```

**SQL script (DDL)**

```sql
CREATE TABLE Members (
    MemberID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100) UNIQUE,
    Phone VARCHAR(20),
    DateJoined DATE
);

CREATE TABLE Books (
    BookID INT PRIMARY KEY,
    Title VARCHAR(150),
    Author VARCHAR(100),
    ISBN VARCHAR(20) UNIQUE,
    PublishedYear INT,
    CopiesAvailable INT
);

CREATE TABLE Loans (
    LoanID INT PRIMARY KEY,
    MemberID INT NOT NULL,
    BookID INT NOT NULL,
    LoanDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    ReturnDate DATE,
    FOREIGN KEY (MemberID) REFERENCES Members(MemberID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID)
);
```

---

## 🌟 What good looks like

- **Every table has a primary key**, and natural-identity columns (`Email`, `ISBN`) carry a `UNIQUE` constraint.
- **Many-to-Many is always resolved with a junction table** (`Enrollments`, `Loans`) holding two foreign keys plus any relationship-specific data (`Grade`, `LoanDate`).
- **Foreign keys reference the correct parent PK**, so the database enforces referential integrity.
- **Column types fit the data:** dates use `DATE`, identifiers use `INT`, text uses sized `VARCHAR`.
- **Nullable columns reflect the domain** — `Grade` and `ReturnDate` are empty until the course finishes or the book is returned.

---

## ☑️ Definition of done

- [ ] University schema lists `Students`, `Professors`, `Courses`, and the `Enrollments` junction table.
- [ ] Library schema lists `Members`, `Books`, and the `Loans` junction table.
- [ ] Every table defines a **primary key**; `Email` and `ISBN` are marked `UNIQUE`.
- [ ] Each **One-to-Many** link (e.g. `Courses.ProfessorID`) is a foreign key to the parent's PK.
- [ ] Each **Many-to-Many** link is resolved by a junction table carrying both foreign keys.
- [ ] Both `CREATE TABLE` scripts run without error and create the tables with their constraints.

---

## 🔑 Key concepts

- **Tables model entities; columns model attributes.** Start by naming the real-world things (students, books) and the facts you store about each.
- **Primary keys give every row a unique identity**, while `UNIQUE` constraints protect natural identifiers like email or ISBN from duplication.
- **One-to-Many is expressed with a foreign key on the "many" side** — a course points to its single professor via `ProfessorID`.
- **Many-to-Many cannot be stored directly; it is resolved with a junction (associative) table** that holds a foreign key to each side and any data about the relationship itself (a grade, a loan date).
- **Foreign keys enforce referential integrity** — the database refuses orphaned rows, keeping the data consistent.
