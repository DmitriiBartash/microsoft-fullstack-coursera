# Capstone Project — Generating Complex SQL Queries

**Course 6 — Database Integration and Management** · Module 4 · `Capstone Activity`

> Build the **SmartShop Inventory System** end-to-end with **Microsoft Copilot** as your
> pair-programmer. Across three activities you'll go from basic `SELECT`s → multi-table
> `JOIN`s, subqueries and aggregation → debugging and performance tuning, finishing with a
> set of high-performing queries for a fictional retailer.

---

## 🎯 Objective

Use Microsoft Copilot to **create, debug, and optimize** SQL queries for SmartShop's
inventory database — retrieving and filtering product data, analyzing sales and supplier
trends across multiple tables, and tuning the final queries so they stay accurate and fast
on large datasets.

---

## 🗂️ What you will produce

A progressively richer query set, one deliverable per activity:

| Activity | Focus | Deliverable |
| -------- | ----- | ----------- |
| **1** | Basic retrieval | `SELECT` + `WHERE` + `ORDER BY` queries over `Products` |
| **2** | Complex analysis | Multi-table `JOIN`s, subqueries, aggregate reports |
| **3** | Debug & optimize | Corrected, indexed, validated final queries |

**Flow:** `Products  →  + Sales + Suppliers (JOIN)  →  subqueries + SUM/AVG/MAX  →  debug  →  index & tune`

> *This is a Copilot-guided exercise — you don't need real rows in the tables. Reason about
> the schema and intent of each query, prompt Copilot to draft the SQL, then read and refine
> what it returns.*

---

## ✅ Prerequisites

- A SQL sandbox environment (e.g. Azure Data Studio, SSMS, or any T-SQL-compatible engine)
- Access to **Microsoft Copilot** to draft and refine queries
- Working knowledge of `SELECT`, `WHERE`, `ORDER BY`, `JOIN`, `GROUP BY`, and subqueries

### The SmartShop schema (reference)

The scenario assumes three core tables. Use this shape when prompting Copilot:

```sql
-- Products: one row per product per store
Products(ProductID, ProductName, Category, Price, StockLevel, StoreLocation, SupplierID)

-- Sales: one row per sale line
Sales(SaleID, ProductID, SaleDate, StoreLocation, UnitsSold)

-- Suppliers: one row per supplier
Suppliers(SupplierID, SupplierName, DeliveredStock, DeliveryDelayDays)
```

---

## 🛠️ Activity 1 — Writing basic SQL queries

> **Scenario.** You are a database engineer building the SmartShop Inventory System for a
> fictional retailer. The system manages inventory across multiple stores and must give
> real-time insight into stock levels, sales trends, and supplier information. SmartShop's
> initial requirements: retrieve product details (name, price, stock), filter by category and
> availability, and sort data for readability.

### Step 1 — Review the scenario

Confirm what Activity 1 must deliver:

- Retrieve product details such as **name, price, and stock levels**.
- Filter products based on **categories and availability**.
- **Sort** data for better readability.

### Step 2 — Generate basic `SELECT` queries with Copilot

Prompt Copilot to write a query returning `ProductName`, `Category`, `Price`, and
`StockLevel`:

```sql
SELECT ProductName,
       Category,
       Price,
       StockLevel
FROM Products;
```

### Step 3 — Implement filtering and sorting

Ask Copilot for queries that filter, then add an `ORDER BY`:

```sql
-- Products in a specific category
SELECT ProductName, Category, Price, StockLevel
FROM Products
WHERE Category = 'Beverages';

-- Products with low stock levels
SELECT ProductName, Category, StockLevel
FROM Products
WHERE StockLevel < 10;

-- Sort all products by Price, ascending
SELECT ProductName, Category, Price, StockLevel
FROM Products
ORDER BY Price ASC;
```

### Step 4 — Save your work

By the end of Activity 1 you will have written basic SQL queries with Copilot to retrieve and
organize inventory data, ready for extension in Activity 2. **Save every query in your sandbox
environment** — they are the starting point for the next activity.

---

## 🔗 Activity 2 — Creating complex SQL queries

> **Recap & new requirements.** With the basics in place, SmartShop now wants to analyze sales
> trends by joining product and sales data, report on supplier performance with aggregate
> functions, and combine data across stores for consolidated reporting — tracking sales by date
> and store, identifying top suppliers by delivered stock, and consolidating inventory.

### Step 1 — Review the recap and new requirements

New needs to satisfy:

- **Track** product sales by date and store.
- **Identify** top-performing suppliers based on delivered stock.
- **Combine** inventory data across stores for consolidated reporting.

### Step 2 — Write multi-table `JOIN` queries with Copilot

Prompt Copilot to join `Products`, `Sales`, and `Suppliers`, displaying `ProductName`,
`SaleDate`, `StoreLocation`, and `UnitsSold`:

```sql
SELECT p.ProductName,
       s.SaleDate,
       s.StoreLocation,
       s.UnitsSold
FROM Sales AS s
JOIN Products  AS p ON p.ProductID  = s.ProductID
JOIN Suppliers AS sup ON sup.SupplierID = p.SupplierID
ORDER BY s.SaleDate, s.StoreLocation;
```

### Step 3 — Implement nested queries and aggregation

Use Copilot to write subqueries and aggregate functions (`SUM`, `AVG`, `MAX`):

```sql
-- Total units sold per product (aggregation)
SELECT p.ProductName,
       SUM(s.UnitsSold) AS TotalUnitsSold
FROM Products AS p
JOIN Sales    AS s ON s.ProductID = p.ProductID
GROUP BY p.ProductName;

-- Suppliers with the most delayed deliveries (subquery filter)
SELECT SupplierName, DeliveryDelayDays
FROM Suppliers
WHERE DeliveryDelayDays = (SELECT MAX(DeliveryDelayDays) FROM Suppliers);

-- Average price per category for trend analysis
SELECT Category,
       AVG(Price) AS AvgPrice,
       MAX(Price) AS MaxPrice
FROM Products
GROUP BY Category;
```

### Step 4 — Save your work

By the end of Activity 2 you will have generated complex SQL queries that meet SmartShop's
advanced requirements — a set ready for debugging and optimization. **Save all complex queries
in your sandbox**; they are debugged and optimized in Activity 3.

---

## 🐛 Activity 3 — Debugging and optimizing SQL queries

> **Recap.** The complex queries from Activity 2 may carry inefficiencies or errors: slow
> execution on large datasets, incorrect `JOIN`/`WHERE` clauses, or inefficient aggregate use.

### Step 1 — Review the recap

Common problems to watch for:

- Slow execution times for large datasets.
- Incorrect `JOIN` or `WHERE` clauses causing errors.
- Inefficient use of aggregate functions.

### Step 2 — Debug errors in SQL queries

Use Copilot to identify and correct errors in `JOIN` statements that produce mismatched
results and nested queries with incorrect syntax. A typical fix — an unqualified or missing
join condition causing a Cartesian product:

```sql
-- Before: missing join predicate explodes rows (accidental CROSS JOIN)
SELECT p.ProductName, s.UnitsSold
FROM Products p, Sales s;

-- After: explicit, correct join key
SELECT p.ProductName, s.UnitsSold
FROM Products p
JOIN Sales s ON s.ProductID = p.ProductID;
```

### Step 3 — Optimize query performance with Copilot

Ask Copilot to suggest and implement optimizations:

- **Add indexes** on frequently queried columns.
- **Restructure** queries for better execution plans.
- **Reduce** unnecessary computations.

```sql
-- Index the columns used in JOIN and WHERE clauses
CREATE INDEX IX_Sales_ProductID    ON Sales(ProductID);
CREATE INDEX IX_Products_Category  ON Products(Category);
CREATE INDEX IX_Products_SupplierID ON Products(SupplierID);
```

### Step 4 — Test and validate the optimized queries

Use Copilot to test and validate:

- **Run** the optimized queries and compare performance with the original versions.
- **Ensure** results are accurate and execution time is reduced.

```sql
-- Inspect the execution plan / timing (SQL Server)
SET STATISTICS TIME ON;
SET STATISTICS IO ON;
-- ...run the optimized query here...
SET STATISTICS TIME OFF;
SET STATISTICS IO OFF;
```

### Step 5 — Save your work

By the end of Activity 3 you will have debugged and optimized the complex queries — a final
set of high-performing queries. **Save them in your sandbox**; they are the final deliverables
for the SmartShop Inventory System.

---

## ▶️ What good looks like

- Activity 1 queries return clean, sorted product data and correctly filter by category and
  stock level.
- Activity 2 `JOIN`s combine `Products`, `Sales`, and `Suppliers` without duplicate or missing
  rows, and aggregates (`SUM`/`AVG`/`MAX`) report the right per-product / per-supplier totals.
- Activity 3 queries produce **identical results** to their Activity 2 versions but run
  **measurably faster** — verified by comparing execution time before and after indexing.

---

## ☑️ Definition of done

- [ ] **Activity 1** — basic `SELECT`, `WHERE` (category + low stock), and `ORDER BY Price ASC` queries saved
- [ ] **Activity 2** — `Products`/`Sales`/`Suppliers` `JOIN` returning `ProductName`, `SaleDate`, `StoreLocation`, `UnitsSold`
- [ ] **Activity 2** — subqueries for per-product totals and most-delayed supplier, plus `SUM`/`AVG`/`MAX` aggregates
- [ ] **Activity 3** — `JOIN`/syntax errors fixed and indexes added to frequently queried columns
- [ ] **Activity 3** — optimized queries validated: same results, reduced execution time
- [ ] All queries saved in the sandbox as the final SmartShop deliverables

---

## 🔑 Key concepts

- **Copilot as a drafting partner** — describe the table, columns, and intent in plain language;
  Copilot drafts the SQL, but you stay responsible for reading, correcting, and validating it.
- **Layered query complexity** — start with `SELECT`/`WHERE`/`ORDER BY`, then build up to
  multi-table `JOIN`s, subqueries, and aggregates as the business questions get harder.
- **Aggregation tells the story** — `SUM`, `AVG`, and `MAX` over `GROUP BY` turn raw sales and
  supplier rows into the trends and rankings SmartShop actually needs.
- **Correctness before speed** — fix mismatched `JOIN`/`WHERE` logic first (an unqualified join is
  an accidental cross product); only then optimize.
- **Indexing for performance** — indexes on `JOIN` keys and filtered columns are the highest-leverage
  tuning move; always confirm the optimized query returns the same results, just faster.
