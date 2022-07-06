using Packt.Shared; // Northwind, Category, Product
using Microsoft.EntityFrameworkCore; //DbSet<T>
using static System.Console;

// FilterAndSort();
// JoinCategoriesAndProducts();
// GroupJoinCategoriesAndProducts();
AggregateProducts();
static void FilterAndSort()
{
    using (Northwind db = new())
    {
        DbSet<Product> allProducts = db.Products;
        IQueryable<Product> filteredProducts =
            allProducts.Where(product => product.UnitPrice < 10M);

        IOrderedQueryable<Product> sortedAndFilteredProducts = 
            filteredProducts.OrderByDescending(product => product.UnitPrice);

        var projectedProducts = sortedAndFilteredProducts
            .Select(product => new // anonymous type
            {
                product.ProductId,
                product.ProductName,
                product.UnitPrice
            });

        WriteLine("Products that cost less than $10:");
        foreach (var p in projectedProducts)
        {
            WriteLine("{0}: {1} costs {2:$#,##0.00}", 
                p.ProductId, p.ProductName, p.UnitPrice);
        }
        WriteLine();
    }
}

static void JoinCategoriesAndProducts()
{
    using (Northwind db = new())
    {
        // Join every product to its category to return 77 matches
        var queryJoin = db.Categories.Join(
            inner: db.Products,
            outerKeySelector: category => category.CategoryId,
            innerKeySelector: product => product.CategoryId,
            resultSelector: (c, p) =>
                new { c.CategoryName, p.ProductName, p.ProductId })
            .OrderBy(cp => cp.CategoryName);

        foreach (var item in queryJoin)
        {
            WriteLine("{0}: {1} is in {2}.",
                item.ProductId,
                item.ProductName,
                item.CategoryName);
        }
    }
}
static void GroupJoinCategoriesAndProducts()
{
    using (Northwind db = new())
    {
        // group all products by their category to return 8 matches
        var queryGroup = db.Categories.AsEnumerable().GroupJoin(
            inner: db.Products,
            outerKeySelector: category => category.CategoryId,
            innerKeySelector: product => product.CategoryId,
            resultSelector: (c, matchingProducts) => new
            {
                c.CategoryName,
                Products = matchingProducts.OrderBy(p => p.ProductName)
            });
    foreach (var category in queryGroup)
        {
            WriteLine("{0} has {1} products.",
                category.CategoryName,
                category.Products.Count());
            foreach (var product in category.Products)
            {
                WriteLine($"    {product.ProductName}");
            }
        }
    }
}
static void AggregateProducts()
{
    using (Northwind db = new())
    {
        WriteLine("{0,-25} {1,10}",
            "Product count:", db.Products.Count());
        WriteLine("{0,-25} {1,10:$#,##0.00}",
            "Highest Product price:", db.Products.Max(p => p.UnitPrice));
        WriteLine("{0,-25} {1,10:N0}",
            "Sum of Units in stock:", db.Products.Sum(p => p.UnitsInStock));
        WriteLine("{0,-25} {1,10:N0}",
            "Sum of units on order:", db.Products.Sum(p => p.UnitsOnOrder));
        WriteLine("{0,-25} {1,10:$#,##0.00}",
            "Average unit price:", db.Products.Average(p => p.UnitPrice));
        WriteLine("{0,-25} {1,10:$#,##0.00}",
            "Value of units in stock:", db.Products.Sum(p => p.UnitPrice * p.UnitsInStock));
    }
}