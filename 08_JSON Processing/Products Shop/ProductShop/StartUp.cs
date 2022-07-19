using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //01:
            var inputJSON1 = File.ReadAllText("..\\..\\..\\Datasets\\users.json");
            Console.WriteLine(ImportUsers(context, inputJSON1));

            //02:
            var inputJSON2 = File.ReadAllText("..\\..\\..\\Datasets\\products.json");
            Console.WriteLine(ImportProducts(context, inputJSON2));

            //03:
            var inputJSON3 = File.ReadAllText("..\\..\\..\\Datasets\\categories.json");
            Console.WriteLine(ImportCategories(context, inputJSON3));

            //04:
            var inputJSON4 = File.ReadAllText("..\\..\\..\\Datasets\\categories-products.json");
            Console.WriteLine(ImportCategoryProducts(context, inputJSON4));

            //05:
            //Console.WriteLine(GetProductsInRange(context));

            //06:
            //Console.WriteLine(GetSoldProducts(context));

            //07:
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //08:
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(inputJson, new JsonSerializerSettings() {
               NullValueHandling = NullValueHandling.Include,
            }).Where(x => x.Name != null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<IEnumerable<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .ToList();

            var productsAsJSON = JsonConvert.SerializeObject(products, Formatting.Indented);

            return productsAsJSON;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var products = context.Users.Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(y => y.BuyerId != null).Select(y => new
                    {
                        name = y.Name,
                        price = y.Price,
                        buyerFirstName = y.Buyer.FirstName,
                        buyerLastName = y.Buyer.LastName
                    }).ToList()
                }).ToList();

            var productsAsJSON = JsonConvert.SerializeObject(products, Formatting.Indented);

            return productsAsJSON;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(x => x.CategoryProducts.Count)
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = $"{(x.CategoryProducts.Select(y => y.Product.Price).Sum() / x.CategoryProducts.Count):F2}",
                    totalRevenue = $"{x.CategoryProducts.Select(y => y.Product.Price).Sum():F2}"
                }).ToList();

            var categoriesAsJSON = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return categoriesAsJSON;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users.Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .Include(x => x.ProductsSold)
                .ToList()
                .OrderByDescending(x => x.ProductsSold.Where(y => y.BuyerId != null).Count())
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(y => y.BuyerId != null).Count(),
                        products = x.ProductsSold.Where(y => y.BuyerId != null).Select(y => new
                        {
                            name = y.Name,
                            price = y.Price
                        }).ToList()
                    }
                }).ToList();

            var usersWithProducts = new
            {
                usersCount = users.Count(),
                users = users
            };

            var usersWithProductsAsJSON = JsonConvert.SerializeObject(usersWithProducts, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return usersWithProductsAsJSON;
        }
    }
}