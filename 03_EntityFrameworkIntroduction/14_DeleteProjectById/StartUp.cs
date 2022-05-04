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
            Console.WriteLine(DeleteProjectById(context));
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Where(x => x.ProjectId == 2).FirstOrDefault();

            var employeesProjects = context.EmployeesProjects.Where(x => x.ProjectId == 2).ToList();

            foreach (var emPr in employeesProjects)
            {
                context.EmployeesProjects.Remove(emPr);
            }

            context.Projects.Remove(project);
            context.SaveChanges();

            var projects = context.Projects.Take(10).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
            }

            return sb.ToString().Trim();
        }
    }
}
