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
            Console.WriteLine(GetEmployeesFullInformation(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            List<string> employeesInfo = context.Employees.OrderBy(x => x.EmployeeId)
                .Select(x => $"{x.FirstName} {x.LastName} {x.MiddleName} {x.JobTitle} {x.Salary:F2}")
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (string employee in employeesInfo)
            {
                sb.AppendLine(employee);
            }

            return sb.ToString().Trim();
        }
    }
}
