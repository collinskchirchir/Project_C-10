using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Packt.Shared;
using static System.Console;
using Microsoft.EntityFrameworkCore; // Include extension method
using Microsoft.EntityFrameworkCore.ChangeTracking; // CollectionEntry



WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider.");
QueryingCategories();
// FilteredIncludes();
// QueryingProducts();
// QueryingWithLike();

static void QueryingCategories()
{
   using (Northwind db = new())
   {
      ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
      loggerFactory.AddProvider(new ConsoleLoggerProvider());
      WriteLine("Categories and how many products they have:");
      
      // a query to get all categories and their related products
      IQueryable<Category>? categories;
         // = db.Categories;
         // .Include(c => c.Products);
      db.ChangeTracker.LazyLoadingEnabled = false;

      Write("Enable eager loading? (Y/N): ");
      bool eagerloading = (ReadKey().Key == ConsoleKey.Y);
      bool explicitloading = false;
      WriteLine();
      if (eagerloading)
      {
         categories = db.Categories?
            .Include(c => c.Products);
      }
      else
      {
         categories = db.Categories;
         WriteLine("Enable explicit loading? (Y/N): ");
         explicitloading = (ReadKey().Key == ConsoleKey.Y);
         WriteLine();
      }
      if(categories is null)
      {
         WriteLine("No categories found.");
         return;
      }
      // execute query and enumerate results
      foreach(Category c in categories)
      {
         if (explicitloading)
         {
            Write($"Explicitly load products for {c.CategoryName}? (Y/N): ");
            ConsoleKeyInfo key = ReadKey();
            WriteLine();
            if (key.Key == ConsoleKey.Y)
            {
               CollectionEntry<Category, Product> products =
                  db.Entry(c).Collection(c2 => c2.Products);
               if (!products.IsLoaded) products.Load();
            }
         }
         WriteLine($"{c.CategoryName} has {c.Products.Count} products.");
      }
   }
}

static void FilteredIncludes()
{
   using(Northwind db = new())
   {
      Write("Enter a minimum for units in stock: ");
      string unitsInStock = ReadLine() ?? "10";
      int stock = int.Parse(unitsInStock);

      IQueryable<Category>? categories = db.Categories?
         .Include(c => c.Products.Where(p => p.Stock >= stock));
      if (categories is null)
      {
         WriteLine("No categories found.");
         return;
      }

      // enumerate the query and output the generated SQL
      WriteLine($"ToQueryString: {categories.ToQueryString()}");

      foreach (Category c in categories)
      {
         WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock.");
         foreach(Product p in c.Products)
         {
            WriteLine($"   {p.ProductName} has {p.Stock} units in stock.");
         }
      }

   }
}

static void QueryingProducts()
{
   using(Northwind db = new())
   {
      ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
      loggerFactory.AddProvider(new ConsoleLoggerProvider());
      WriteLine("Products that cost more than a price, highest at top.");
      string? input;
      decimal price;
      do
      {
         Write("Enter a product price: ");
         input = ReadLine();

      }
      while(!decimal.TryParse(input, out price));

      IQueryable<Product>? products = db.Products?
         .TagWith("Products filtered by price and sorted.")
         .Where(product => product.Cost > price)
         .OrderByDescending(product => product.Cost);
      if(products is null)
      {
         WriteLine("No products found.");
         return;
      }
      foreach(Product p in products)
      {
         WriteLine("{0}: {1} costs {2:$#,##0.00} and has {3} in stock.",
            p.ProductId, p.ProductName, p.Cost, p.Stock);
      }
   
   }
}
static void QueryingWithLike()
{
   using (Northwind db = new())
   {
      ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
      loggerFactory.AddProvider(new ConsoleLoggerProvider());

      Write("Enter part of the product name: ");
      string? input = ReadLine();

      IQueryable<Product>? products = db.Products?
         .Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));
      
      if (products is null)
      {
         WriteLine("No products found.");
         return;
      }
      foreach (Product p in products)
      {
         WriteLine("{0} has {1} units in stock. Discontinued? {2}", 
            p.ProductName, p.Stock, p.Discontinued);
      }
   }
}