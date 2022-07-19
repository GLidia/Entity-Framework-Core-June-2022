using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //09:
            var inputJson9 = File.ReadAllText("..\\..\\..\\Datasets\\suppliers.json");
            Console.WriteLine(ImportSuppliers(context, inputJson9));

            //10:
            var inputJson10 = File.ReadAllText("..\\..\\..\\Datasets\\parts.json");
            Console.WriteLine(ImportParts(context, inputJson10));

            //11:
            var inputJson11 = File.ReadAllText("..\\..\\..\\Datasets\\cars.json");
            Console.WriteLine(ImportCars(context, inputJson11));

            //12:
            var inputJson12 = File.ReadAllText("..\\..\\..\\Datasets\\customers.json");
            Console.WriteLine(ImportCustomers(context, inputJson12));

            //13:
            var inputJson13 = File.ReadAllText("..\\..\\..\\Datasets\\sales.json");
            Console.WriteLine(ImportSales(context, inputJson13));

            //14:
            //Console.WriteLine(GetOrderedCustomers(context));

            //15:
            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //16:
            //Console.WriteLine(GetLocalSuppliers(context));

            //17:
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //18:
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            //19:
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<IEnumerable<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<IEnumerable<Part>>(inputJson).Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportCarsInputModel>>(inputJson);
            var cars = new List<Car>();

            foreach (var carDTO in carDTOs)
            {
                Car car = new Car()
                {
                    Make = carDTO.Make,
                    Model = carDTO.Model,
                    TravelledDistance = carDTO.TravelledDistance
                };

                foreach(var partId in carDTO.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        PartId = partId
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<IEnumerable<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            var customersAsJSON = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return customersAsJSON;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            var carsAsJSON = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return carsAsJSON;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToList();

            var localSuppliersAsJSON = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);

            return localSuppliersAsJSON;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars.Select(y => y.Part).Select(z => new
                    {
                        Name = z.Name,
                        Price = $"{z.Price:F2}"
                    }).ToList()
                }).ToList();

            var carsWithPartsAsJSON = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);
            return carsWithPartsAsJSON;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context) 
        {
            var customersWithCars = context.Customers.Where(x => x.Sales.Count > 0)
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    spentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var customersWithCarsAsJSON = JsonConvert.SerializeObject(customersWithCars, Formatting.Indented);

            return customersWithCarsAsJSON;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Select(x => new
            {
                car = new
                {
                    Make = x.Car.Make,
                    Model = x.Car.Model,
                    TravelledDistance = x.Car.TravelledDistance
                },
                customerName = x.Customer.Name,
                Discount = $"{x.Discount:F2}",
                price = $"{(x.Car.PartCars.Sum(y => y.Part.Price)):F2}",
                priceWithDiscount = $"{(x.Car.PartCars.Sum(y => y.Part.Price) * (100 - x.Discount) / 100):F2}"
            }).ToList().Take(10);

            var salesAsJSON = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return salesAsJSON;
        }
    }
}