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
            Console.WriteLine(GetEmployee147(context));
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.Where(x => x.EmployeeId == 147)
                .Select(x => new { x.FirstName, x.LastName, x.JobTitle, 
                    Projects = x.EmployeesProjects.OrderBy(y => y.Project.Name).Select(y => y.Project.Name).ToList()})
                .FirstOrDefault();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var projectName in employee.Projects)
            {
                sb.AppendLine($"{projectName}");
            }

            return sb.ToString().Trim();
        }
    }
}
