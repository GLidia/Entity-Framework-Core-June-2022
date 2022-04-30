
using MiniORM.App.Data;
using MiniORM.App.Data.Entities;

var connectionString = "Server=.\\SQLExpress;Database=MiniORM;Integrated Security=True";

var context = new SoftUniDbContext(connectionString);

context.Employees.Add(new Employee
{
    FirstName = "Gosho",
    LastName = "Inserted",
    DepartmentId = context.Departments.First().Id,
    IsEmployed = true,
});

var employee = context.Employees.Last();
employee.FirstName = "Modified";

context.SaveChanges();
