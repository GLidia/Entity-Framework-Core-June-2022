namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var restriction = Enum.Parse<AgeRestriction>(command, true);

            var bookTitles = context.Books.Where(x => x.AgeRestriction == restriction).Select(x => x.Title).OrderBy(x => x).ToList();

            var sb = new StringBuilder();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().Trim();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books.Where(x => x.EditionType == EditionType.Gold).Where(x => x.Copies < 5000)
                .OrderBy(x => x.BookId).Select(x => x.Title).ToList();

            var sb = new StringBuilder();

            foreach(var title in goldenBooks)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Price > 40).Select(x => new
            {
                x.Title,
                x.Price,
            }).OrderByDescending(x => x.Price).ToList();

            var sb = new StringBuilder();

            foreach(var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books.Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId).Select(x => x.Title).ToList();

            var sb = new StringBuilder();

            foreach(var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToList();

            var bookTitles = context.BooksCategories.Where(x => categories.Contains(x.Category.Name.ToLower()))
                .Select(x => x.Book.Title).OrderBy(x => x).ToList();

            var sb = new StringBuilder();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var formattedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books.Where(x => x.ReleaseDate.Value < formattedDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price,
                }).ToList();

            var sb = new StringBuilder();

            foreach(var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors.Where(x => x.FirstName.EndsWith(input))
                .Select(x => x.FirstName + " " + x.LastName).OrderBy(x => x).ToList();

            var sb = new StringBuilder();

            foreach(var author in authors)
            {
                sb.AppendLine(author);
            }

            return sb.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books.Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title).OrderBy(x => x).ToList();

            var sb = new StringBuilder();

            foreach(var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var bookTitlesAndAuthors = context.Books.Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId).Select(x => new
                {
                    x.Title,
                    AuthorName = x.Author.FirstName + " " + x.Author.LastName
                }).ToList();

            var sb = new StringBuilder();

            foreach(var item in bookTitlesAndAuthors)
            {
                sb.AppendLine($"{item.Title} ({item.AuthorName})");
            }

            return sb.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books.Where(x => x.Title.Length > lengthCheck).Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsAndBookCopies = context.Authors.Select(x => new
            {
                AuthorName = x.FirstName + " " + x.LastName,
                CopiesCount = x.Books.Select(y => y.Copies).Sum()
            }).OrderByDescending(x => x.CopiesCount).ToList();

            var sb = new StringBuilder();

            foreach(var item in authorsAndBookCopies)
            {
                sb.AppendLine($"{item.AuthorName} - {item.CopiesCount}");
            }

            return sb.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesAndProfit = context.Categories.Select(x => new
            {
                Category = x.Name,
                TotalProfit = x.CategoryBooks.Select(x => x.Book.Price * x.Book.Copies).Sum()
            }).OrderByDescending(x => x.TotalProfit).ThenBy(x => x.Category).ToList();

            var sb = new StringBuilder();

            foreach(var c in categoriesAndProfit)
            {
                sb.AppendLine($"{c.Category} ${c.TotalProfit:F2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var mostRecentBooksByCategory = context.Categories.Select(x => new
            {
                Category = x.Name,
                TopThreeBooks = x.CategoryBooks.OrderByDescending(y => y.Book.ReleaseDate).Select(y => new
                {
                    y.Book.Title,
                    y.Book.ReleaseDate
                }).Take(3).ToList()
            }).OrderBy(x => x.Category).ToList();

            var sb = new StringBuilder();

            foreach(var c in mostRecentBooksByCategory)
            {
                sb.AppendLine($"--{c.Category}");
                foreach(var book in c.TopThreeBooks)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010).ToList();

            foreach(var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Copies < 4200).ToList();

            var booksCount = books.Count();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return booksCount;
        }
    }
}

