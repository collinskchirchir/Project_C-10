using Packt.Shared;
using static System.Console;
using Microsoft.EntityFrameworkCore; // Include extension method



WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider.");
QueryingCategories();

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
      // execute query and enumarate results
      foreach(Category c in categories)
      {
         WriteLine($"{c.CategoryName} has {c.Products.Count} products.");
      }
   }
}