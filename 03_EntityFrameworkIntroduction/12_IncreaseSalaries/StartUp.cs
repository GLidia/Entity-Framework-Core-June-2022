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
            Console.WriteLine(IncreaseSalaries(context));
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees.Where(x => x.Department.Name == "Engineering" || x.Department.Name == "Tool Design"
                || x.Department.Name == "Marketing" || x.Department.Name == "Information Services")
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .ToList();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
            }

            return sb.ToString().Trim();
        }
    }
}
