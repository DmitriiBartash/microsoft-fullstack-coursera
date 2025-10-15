-- ============================================
-- SMARTSHOP INVENTORY SYSTEM PROJECT
-- Activities 1–3 (Full SQL Project)
-- ============================================

-- ============================================
-- ACTIVITY 1: Writing Basic SQL Queries
-- ============================================

CREATE DATABASE IF NOT EXISTS SmartShopDB;
USE SmartShopDB;

-- Clean start
DROP TABLE IF EXISTS Products;

CREATE TABLE Products (
    ProductID INT AUTO_INCREMENT PRIMARY KEY,
    ProductName VARCHAR(100),
    Category VARCHAR(50),
    Price DECIMAL(10,2),
    StockLevel INT
);

-- Insert sample product data
INSERT INTO Products (ProductName, Category, Price, StockLevel)
VALUES
('Laptop Pro', 'Electronics', 1200.00, 8),
('Wireless Mouse', 'Accessories', 25.00, 45),
('Office Chair', 'Furniture', 320.00, 5),
('Bluetooth Speaker', 'Electronics', 95.00, 20),
('Desk Lamp', 'Furniture', 40.00, 0),
('Smartphone X', 'Electronics', 980.00, 12);

-- Retrieve product details
SELECT ProductName, Category, Price, StockLevel
FROM Products;

-- Filter: products in a specific category
SELECT * FROM Products
WHERE Category = 'Electronics';

-- Filter: low stock items (< 10)
SELECT ProductName, StockLevel
FROM Products
WHERE StockLevel < 10;

-- Sort: products by price ascending
SELECT ProductName, Price
FROM Products
ORDER BY Price ASC;

-- ============================================
-- ACTIVITY 2: Complex Queries with Joins & Aggregation
-- ============================================

-- Drop old tables if exist
DROP TABLE IF EXISTS Suppliers;
DROP TABLE IF EXISTS Sales;

CREATE TABLE Suppliers (
    SupplierID INT AUTO_INCREMENT PRIMARY KEY,
    SupplierName VARCHAR(100),
    Country VARCHAR(50)
);

CREATE TABLE Sales (
    SaleID INT AUTO_INCREMENT PRIMARY KEY,
    ProductID INT,
    StoreLocation VARCHAR(100),
    SaleDate DATE,
    UnitsSold INT,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Insert sample supplier data
INSERT INTO Suppliers (SupplierName, Country)
VALUES
('TechWorld', 'USA'),
('OfficeDepot', 'Germany'),
('FurniHouse', 'Italy');

-- Insert sample sales data
INSERT INTO Sales (ProductID, StoreLocation, SaleDate, UnitsSold)
VALUES
(1, 'New York', '2024-09-01', 4),
(1, 'Chicago', '2024-09-02', 2),
(2, 'Berlin', '2024-09-01', 10),
(3, 'Rome', '2024-09-03', 3),
(4, 'Paris', '2024-09-01', 7),
(6, 'New York', '2024-09-02', 5);

-- Join: show product sales with store and date
SELECT 
    P.ProductName,
    S.SaleDate,
    S.StoreLocation,
    S.UnitsSold
FROM Products P
JOIN Sales S ON P.ProductID = S.ProductID;

-- Subquery: total sales for each product
SELECT 
    ProductName,
    (SELECT SUM(UnitsSold) FROM Sales WHERE ProductID = P.ProductID) AS TotalUnitsSold
FROM Products P;

-- Aggregation: total and average units sold per category
SELECT 
    P.Category,
    SUM(S.UnitsSold) AS TotalUnits,
    AVG(S.UnitsSold) AS AvgUnits
FROM Sales S
JOIN Products P ON S.ProductID = P.ProductID
GROUP BY P.Category;

-- Example: supplier performance (simulated)
SELECT SupplierName, COUNT(*) AS TotalDeliveries
FROM Suppliers
GROUP BY SupplierName;

-- ============================================
-- ACTIVITY 3: Debugging and Optimization
-- ============================================

-- Fix JOINs
SELECT P.ProductName, S.StoreLocation, S.UnitsSold
FROM Products P
JOIN Sales S ON P.ProductID = S.ProductID;

-- Fix nested query syntax
SELECT ProductName
FROM Products
WHERE ProductID IN (
    SELECT ProductID FROM Sales WHERE UnitsSold > 3
);

-- Create indexes to optimize frequent queries
CREATE INDEX idx_category ON Products(Category);
CREATE INDEX idx_sales_date ON Sales(SaleDate);
CREATE INDEX idx_product_id ON Sales(ProductID);

-- Optimized aggregation
SELECT P.Category, SUM(S.UnitsSold) AS TotalSales
FROM Products P
JOIN Sales S ON P.ProductID = S.ProductID
WHERE S.SaleDate >= '2024-09-01'
GROUP BY P.Category;

-- Analyze query performance
EXPLAIN SELECT P.ProductName, SUM(S.UnitsSold)
FROM Products P
JOIN Sales S ON P.ProductID = S.ProductID
GROUP BY P.ProductName;

-- ============================================
-- Final Summary Report
-- ============================================

SELECT 
    P.Category,
    COUNT(DISTINCT P.ProductID) AS ProductCount,
    SUM(S.UnitsSold) AS TotalUnitsSold,
    ROUND(AVG(S.UnitsSold), 2) AS AvgUnitsSold,
    SUM(P.Price * S.UnitsSold) AS TotalRevenue
FROM Products P
JOIN Sales S ON P.ProductID = S.ProductID
GROUP BY P.Category
ORDER BY TotalRevenue DESC;

-- ============================================
-- END OF SMARTSHOP PROJECT
-- ============================================
