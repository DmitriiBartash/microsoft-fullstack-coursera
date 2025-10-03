﻿using EFCoreModelApp.Data;
using EFCoreModelApp.Models;

using var context = new HRDbContext();

context.Database.EnsureCreated();

void PrintTable<T>(IEnumerable<T> items, string title)
{
    Console.WriteLine($"\n{title}");
    Console.WriteLine(new string('-', 100));

    var props = typeof(T).GetProperties();

    foreach (var prop in props)
        Console.Write($"{prop.Name,-30}");
    Console.WriteLine();
    Console.WriteLine(new string('-', 100));

    foreach (var item in items)
    {
        foreach (var prop in props)
        {
            var value = prop.GetValue(item) ?? "";
            Console.Write($"{value,-30}");
        }
        Console.WriteLine();
    }

    Console.WriteLine(new string('-', 100));
}

var employees = context.Employees
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();

PrintTable(employees, "All Employees");

var hrEmployees = context.Employees
    .Where(e => e.Department.Name == "HR")
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();

PrintTable(hrEmployees, "Employees in HR Department");

if (!context.Employees.Any(e => e.Name == "Charlie"))
{
    var newEmployee = new Employee
    {
        Name = "Charlie",
        Position = "System Administrator",
        DepartmentId = 2
    };

    context.Employees.Add(newEmployee);
    context.SaveChanges();
    Console.WriteLine("\nAdded new employee: Charlie (System Administrator, IT).");
}
else
{
    Console.WriteLine("\nCharlie already exists. Skipping insert.");
}

var updatedEmployees = context.Employees
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();

PrintTable(updatedEmployees, "Updated Employees List");

var departments = context.Departments
    .Select(d => new { d.DepartmentId, d.Name })
    .ToList();

PrintTable(departments, "Departments");
