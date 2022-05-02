using SoftUni.Data;
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
            Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees.Where(x => x.Department.Name == "Research and Development")
                .Select(x => new { x.FirstName, x.LastName, Department = x.Department.Name, x.Salary })
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department} - ${employee.Salary:F2}");
            }

            return sb.ToString().Trim();
        }
    }
}
