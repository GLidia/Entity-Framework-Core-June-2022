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
            Console.WriteLine(GetAddressesByTown(context));
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(x => new { x.AddressText, TownName = x.Town.Name, NumberOfEmployees = x.Employees.Count() })
                .OrderByDescending(x => x.NumberOfEmployees).ThenBy(x => x.TownName).ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.NumberOfEmployees} employees");
            }

            return sb.ToString().Trim();
        }
    }
}
