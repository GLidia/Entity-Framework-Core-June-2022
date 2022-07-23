using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //1:
            var inputXML1 = File.ReadAllText("..\\..\\..\\Datasets\\users.xml");
            Console.WriteLine(ImportUsers(context, inputXML1));

            //2:
            var inputXML2 = File.ReadAllText("..\\..\\..\\Datasets\\products.xml");
            Console.WriteLine(ImportProducts(context, inputXML2));

            //3:
            var inputXML3 = File.ReadAllText("..\\..\\..\\Datasets\\categories.xml");
            Console.WriteLine(ImportCategories(context, inputXML3));

            //4:
            var inputXML4 = File.ReadAllText("..\\..\\..\\Datasets\\categories-products.xml");
            Console.WriteLine(ImportCategoryProducts(context, inputXML4));

            //5:
            //Console.WriteLine(GetProductsInRange(context));

            //6:
            //Console.WriteLine(GetSoldProducts(context));

            //7:
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //8:
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<UserInputModel>), new XmlRootAttribute("Users"));
            var deserializedUsers = (List<UserInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var users = deserializedUsers.Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age
            }).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ProductInputModel>), new XmlRootAttribute("Products"));
            var deserializedProducts = (List<ProductInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var products = deserializedProducts.Select(x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                SellerId = x.SellerId,
                BuyerId = x.BuyerId
            }).ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}"; 
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoryInputModel>), new XmlRootAttribute("Categories"));
            var deserializedCategories = (List<CategoryInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var categories = deserializedCategories.Where(x => x.Name != null).Select(x => new Category
            {
                Name = x.Name
            }).ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoryProductInputModel>), new XmlRootAttribute("CategoryProducts"));
            var deserializedCategoryProducts = (List<CategoryProductInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = deserializedCategoryProducts
                .Where(x => context.Categories.Any(y => y.Id == x.CategoryId) && context.Products.Any(y => y.Id == x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId
                }).ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new ProductsInRangeOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerFullName = x.Buyer.FirstName + " " + x.Buyer.LastName
                }).Take(10).ToList();

            var serializer = new XmlSerializer(typeof(List<ProductsInRangeOutputModel>), new XmlRootAttribute("Products"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(textWriter, products, ns);

            return textWriter.ToString();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users.Where(x => x.ProductsSold.Count > 0)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .Select(x => new UsersSoldProductsOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProductsSold = x.ProductsSold.Select(y => new ProductOutputModel
                    {
                        Name = y.Name,
                        Price = y.Price
                    }).ToArray()
                }).Take(5).ToList();

            var serializer = new XmlSerializer(typeof(List<UsersSoldProductsOutputModel>), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(textWriter, usersWithProducts, ns);

            return textWriter.ToString();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new CategoriesOutputModel
                {
                    Name = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Select(y => y.Product.Price).Average(),
                    TotalRevenue = x.CategoryProducts.Select(y => y.Product.Price).Sum()
                })
                .OrderByDescending(x => x.ProductsCount)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CategoriesOutputModel>), new XmlRootAttribute("Categories"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(textWriter, categories, ns);

            return textWriter.ToString();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users.Where(x => x.ProductsSold.Count > 0).OrderByDescending(x => x.ProductsSold.Count)
                .ToList()
                .Select(x => new UsersWithProductsOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new SoldProductsOutputModel
                    {
                        ProductsCount = x.ProductsSold.Count(),
                        Products = x.ProductsSold.Select(y => new ProductOutputModel
                        {
                            Name = y.Name,
                            Price = y.Price
                        }).OrderByDescending(y => y.Price).ToArray()
                    }
                }).ToArray();                

            var users = new OverarchingUsersWithProductsOutputModel
            {
                Count = usersWithProducts.Count(),
                UsersWithProducts = usersWithProducts.Take(10).ToArray()
            };

            var serializer = new XmlSerializer(typeof(OverarchingUsersWithProductsOutputModel), new XmlRootAttribute(""));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(textWriter, users, ns);

            return textWriter.ToString();
        }
    }
}