using Packt.Shared;
using static System.Console;
using Microsoft.EntityFrameworkCore; // Include extension method



WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider.");
// QueryingCategories();
FilteredIncludes();

static void QueryingCategories()
{
   using (Northwind db = new())
   {
      WriteLine("Categories and how many products they have:");
      
      // a query to get all categories and their related products
      IQueryable<Category>? categories = db.Categories?
         .Include(c => c.Products);
      if(categories is null)
      {
         WriteLine("No categories found.");
         return;
      }
      // execute query and enumerate results
      foreach(Category c in categories)
      {
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