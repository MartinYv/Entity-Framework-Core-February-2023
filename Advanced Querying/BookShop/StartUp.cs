namespace BookShop
{
	using BookShop.Models.Enums;
	using Data;
	using Initializer;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions;
	using System.Runtime.CompilerServices;
	using System.Security.Cryptography.X509Certificates;
	using System.Text;

	public class StartUp
	{
		public static void Main()
		{
			using var db = new BookShopContext();
			// DbInitializer.ResetDatabase(db);

			string command = Console.ReadLine();

			Console.WriteLine(GetBooksReleasedBefore(db, command));
		}

		public static string GetBooksByAgeRestriction(BookShopContext context, string command)
		{
			bool hasParsed = Enum.TryParse(typeof(AgeRestriction), command, true, out object? ageRestrictionObject);
			AgeRestriction ageRestriction;

			if (hasParsed)
			{
				ageRestriction = (AgeRestriction)ageRestrictionObject;


				string[] bookTitles = context.Books
						.Where(b => b.AgeRestriction == ageRestriction)
						.OrderBy(b => b.Title)
						.Select(t => t.Title)
						.ToArray();

				return string.Join(Environment.NewLine, bookTitles);
			}

			return null;
		}

		public static string GetGoldenBooks(BookShopContext context)
		{

			string[] goldenBooks = context.Books.Where(bc => bc.Copies < 5000 && bc.EditionType == EditionType.Gold)
				.OrderBy(b => b.BookId)
				.Select(bt => bt.Title)
				.ToArray();

			return string.Join(Environment.NewLine, goldenBooks);

		}

		public static string GetBooksByPrice(BookShopContext context)
		{
			var bookTitlesByPrice = context.Books.Where(b => b.Price > 40)
				.OrderByDescending(b => b.Price)
				.Select(b => new
				{
					b.Title,
					b.Price
				})
				.ToArray();

			StringBuilder output = new StringBuilder();

			foreach (var item in bookTitlesByPrice)
			{
				output.AppendLine($"{item.Title} - {item.Price}");
			}

			return output.ToString().TrimEnd();
		}

		public static string GetBooksByCategory(BookShopContext context, string input)
		{
			string[] categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToLower()).ToArray();


			string[] booksByCategories = context.Books
				.Where(b => b.BookCategories
				.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
				.OrderBy(b => b.Title)
				.Select(b => b.Title)
				.ToArray();


			return string.Join(Environment.NewLine, booksByCategories);
		}

		//07
		public static string GetBooksReleasedBefore(BookShopContext context, string date)
		{
			//int year = int.Parse(DateTime.Parse(date).ToString()); take the year


			DateTime year = DateTime.Parse(date);

			var selected = context.Books.Where(x => x.ReleaseDate > year) // TODO
				.Select(x => new
				{
					Title = x.Title,
					EditionType = x.EditionType.ToString(),
					Price = x.Price.ToString()
				}).ToArray();

			StringBuilder output = new StringBuilder();

			foreach (var book in selected)
			{
				output.AppendLine($"{book.Title} - {book.EditionType} - {book.Price}");
			}

			return output.ToString().TrimEnd();

		}

		public static string GetBookTitlesContaining(BookShopContext context, string input)
		{
			var sb = new StringBuilder();

			var books = context.Books
				.Where(b => b.Title.ToLower().Contains(input.ToLower()))
				.OrderBy(b => b.Title);

			foreach (var book in books)
			{
				sb.AppendLine(book.Title);
			}

			return sb.ToString().Trim();
		}

		public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
		{
			var sb = new StringBuilder();

			var authors = context.Authors
				.Where(a => a.FirstName.EndsWith(input));

			foreach (var author in authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName))
			{
				sb.AppendLine($"{author.FirstName} {author.LastName}");
			}


			return sb.ToString().Trim();
		}

		public static string GetBooksReleasedBefore(BookShopContext context, string date)
		{
			var sb = new StringBuilder();

			var books = context.Books
				.Select(b => new
				{
					Title = b.Title,
					EditionType = b.EditionType,
					Price = b.Price,
					ReleaseDate = b.ReleaseDate
				})
				.Where(b => b.ReleaseDate!.Value.Date < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
				.OrderByDescending(b => b.ReleaseDate);


			foreach (var book in books)
			{
				sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
			}

			return sb.ToString().Trim();
		}

		public static string GetBooksByCategory(BookShopContext context, string input)
		{
			var sb = new StringBuilder();

			var bookList = new List<string>();

			var categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

			var books = context.BooksCategories
				.Select(b => new
				{
					BookTitle = b.Book.Title,
					BookCategory = b.Category.Name
				});

			foreach (var category in categories)
			{
				foreach (var book in books.Where(b => b.BookCategory.ToLower() == category.ToLower()))
				{
					bookList.Add(book.BookTitle);
				}
			}

			sb.AppendLine(string.Join(Environment.NewLine, bookList.OrderBy(b => b)));

			return sb.ToString().Trim();
		}

		public static string GetBooksNotReleasedIn(BookShopContext context, int year)
		{
			var sb = new StringBuilder();

			var books = context.Books
				.Where(b => b.ReleaseDate!.Value.Year != year)
				.OrderBy(b => b.BookId);

			foreach (var book in books)
			{
				sb.AppendLine(book.Title);
			}

			return sb.ToString().Trim();
		}

		public static string GetBooksByPrice(BookShopContext context)
		{
			var sb = new StringBuilder();

			var books = context.Books
				.Where(b => b.Price > 40)
				.OrderByDescending(p => p.Price);

			foreach (var book in books)
			{
				sb.AppendLine($"{book.Title} - ${book.Price:f2}");
			}

			return sb.ToString().Trim();
		}

		public static string GetGoldenBooks(BookShopContext context)
		{
			var sb = new StringBuilder();

			var books = context.Books
				.Where(b => b.Copies < 5000)
				.OrderBy(b => b.BookId);

			foreach (var book in books.Where(b => b.EditionType.Equals(EditionType.Gold)))
			{
				sb.AppendLine(book.Title);
			}

			return sb.ToString().Trim();
		}

		public static string GetBooksByAgeRestriction(BookShopContext context, string command)
		{
			var sb = new StringBuilder();

			var books = context.Books.ToList();

			foreach (var book in books.Where(b => b.AgeRestriction.ToString().ToLower().Equals(command.ToLower())).OrderBy(b => b.Title))
			{
				sb.AppendLine(book.Title);
			}

			return sb.ToString().Trim();
		}
	}
}