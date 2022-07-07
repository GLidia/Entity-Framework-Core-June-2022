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
            Console.WriteLine(RemoveTown(context));
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var employeesInSeattle = context.Employees.Where(x => x.Address.Town.Name == "Seattle").ToList();

            foreach (var e in employeesInSeattle)
            {
                e.AddressId = null;
            }

            var addresses = context.Addresses.Where(x => x.Town.Name == "Seattle").ToList();
            var addressesCount = addresses.Count();

            foreach (var a in addresses)
            {
                context.Addresses.Remove(a);
            }

            var town = context.Towns.Where(x => x.Name == "Seattle").FirstOrDefault();
            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{addressesCount} addresses in Seattle were deleted";
        }
    }
}
