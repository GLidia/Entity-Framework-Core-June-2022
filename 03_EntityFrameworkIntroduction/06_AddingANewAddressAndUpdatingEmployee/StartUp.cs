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
            Console.WriteLine(AddNewAddressToEmployee(context));
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var newAddress = new Address() { AddressText = "Vitoshka 15", TownId = 4 };
            context.Addresses.Add(newAddress);

            var employeeNakov = context.Employees.Where(x => x.LastName == "Nakov").FirstOrDefault();
            employeeNakov.Address = newAddress;

            context.SaveChanges();
            
            var employeesAddresses = context.Employees
                .OrderByDescending(x => x.Address.AddressId)
                .Select(x => new { x.Address.AddressText})
                .ToList().Take(10);

            StringBuilder sb = new StringBuilder();

            foreach (var address in employeesAddresses)
            {
                sb.AppendLine($"{address.AddressText}");
            }

            return sb.ToString().Trim();
        }
    }
}
