using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments.Where(x => x.Employees.Count() > 5)
                .OrderBy(x => x.Employees.Count()).ThenBy(x => x.Name)
                .Select(x => new { DepartmentName = x.Name, ManagerFirstName = x.Manager.FirstName, ManagerLastName = x.Manager.LastName,
                    DepartmentEmployees = x.Employees
                    .Select(y => new { EmployeeFirstName = y.FirstName, EmployeeLastName = y.LastName, EmployeeJobTitle = y.JobTitle })
                    .OrderBy(y => y.EmployeeFirstName).ThenBy(y => y.EmployeeLastName)
                    .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepartmentName} - {department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.DepartmentEmployees)
                {
                    sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.EmployeeJobTitle}");
                }
            }

            return sb.ToString().Trim();
        }
    }
}
