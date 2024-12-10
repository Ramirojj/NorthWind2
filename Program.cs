using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";
// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();
logger.Info("Program started");
Console.WriteLine("Hello World!");


do
{
  Console.WriteLine("1) Display categories");
  Console.WriteLine("2) Add category");
  Console.WriteLine("3) Display Category and related products");
  Console.WriteLine("4) Display all Categories and their related products");
    Console.WriteLine("5) Delete category");
  Console.WriteLine("6) Add product");
  Console.WriteLine("7) Edit a specified record from the Products table");
Console.WriteLine("8) Display all records in the Products table");
Console.WriteLine("9) Display a specific Product (all product fields should be displayed)");
Console.WriteLine("10) Use NLog to track user functions");
Console.WriteLine("11)Add new records to the Categories table ");
Console.WriteLine("12)Edit a specified record from the Categories table ");
Console.WriteLine("13)Display all Categories in the Categories table  ");
  Console.WriteLine("Enter to quit");
  ;
  string? choice = Console.ReadLine();
  Console.Clear();
  logger.Info("Option {choice} selected", choice);
  if (choice == "1")
  {
   

// display categories
    var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
    var config = configuration.Build();
    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryName);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ForegroundColor = ConsoleColor.White;



  }
  else if (choice == "2")
  {
    

Category category = new();
    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine()!;
    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();
    // TODO: save category to db
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
      var db = new DataContext();
      // check for unique name
      if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
      {
        // generate validation error
        isValid = false;
        results.Add(new ValidationResult("Name exists", ["CategoryName"]));
      }
      else
      {
        logger.Info("Validation passed");
        // TODO: save category to db
      }
    }
    if (!isValid)
    {
      foreach (var result in results)
      {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
      }
    }





  }
  else if (choice == "3")
  {
    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryId);
    Console.WriteLine("Select the category whose products you want to display:");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ForegroundColor = ConsoleColor.White;
    int id = int.Parse(Console.ReadLine()!);
    Console.Clear();
    logger.Info($"CategoryId {id} selected");
    Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id)!;
    Console.WriteLine($"{category.CategoryName} - {category.Description}");

foreach (Product p in category.Products)
    {
     Console.WriteLine($"\t{p.ProductName}");
    }


  }
  else if (choice == "4")
  {
    var db = new DataContext();
    var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName}");
      foreach (Product p in item.Products)
      {
        Console.WriteLine($"\t{p.ProductName}");
      }
    }
  }
////

///

else if (choice == "6")
{
var db = new DataContext();
    Product product = new();
    Console.WriteLine("Enter Product Name:");
    product.ProductName = Console.ReadLine()!;
    Console.WriteLine("Enter Unit Price (or leave blank):");
    product.UnitPrice = decimal.TryParse(Console.ReadLine(), out decimal unitPrice) ? unitPrice : null;
    Console.WriteLine("Enter Units In Stock (or leave blank):");
    product.UnitsInStock = short.TryParse(Console.ReadLine(), out short unitsInStock) ? unitsInStock : null;
    Console.WriteLine("Is the product discontinued? (y/n):");
    product.Discontinued = Console.ReadLine()?.ToLower() == "y";
    if (!db.Products.Any(p => p.ProductName == product.ProductName))
    {
        db.Products.Add(product);
        db.SaveChanges();
        Console.WriteLine("Product added successfully.");}
    else
    {
        Console.WriteLine("A product with this name already exists.");
    }}

/////

else if (choice == "7")
{
    var db = new DataContext();

    Console.WriteLine("Enter the ID of the product to edit:");
    if (int.TryParse(Console.ReadLine(), out int productId))
    {
        var product = db.Products.Find(productId);

        if (product != null){
        
            Console.WriteLine($"Editing {product.ProductName}...");
            Console.WriteLine("New Product Name :");
            string? newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName)) product.ProductName =   newName;
            Console.WriteLine("New Unit Price:");
            if (decimal.TryParse(Console.ReadLine(), out decimal newPrice)) product.UnitPrice = newPrice;


            Console.WriteLine("New Units In Stock :");
            if (short.TryParse(Console.ReadLine(), out short newStock)) product.UnitsInStock = newStock;

            Console.WriteLine("Discontinued? :");
            string? discontinued = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(discontinued)) product.Discontinued = discontinued.ToLower() == "y";

            db.SaveChanges();
            Console.WriteLine("product updated.");
                     }  }
   
}

/////
else if (choice == "8")
{
    var db = new DataContext();
    Console.WriteLine("Select the type of products to display:");
    Console.WriteLine("1) All products");
    Console.WriteLine("2) Discontinued products");

    Console.WriteLine("3) Active  products ? ");

    string? productChoice = Console.ReadLine();
    IQueryable<Product> query = db.Products;
    if (productChoice == "1")
    {
        Console.WriteLine("All products:");
    }
    else if (productChoice == "2")
    {
        query = query.Where(p => p.Discontinued);
        Console.WriteLine("Discontinued products:");
    }
    else if (productChoice == "3")
    {
        query = query.Where(p => !p.Discontinued);
        Console.WriteLine("Active products:");
    }
    else
    {
        Console.WriteLine("Invalid choice.");
        return;
    }

    foreach (var product in query.OrderBy(p => p.ProductName))
    {
        string status = product.Discontinued ? "(Discontinued)" : "(Active)";
        Console.WriteLine($"{product.ProductName} {status}");
    }
}
///
else if (choice == "9")
{
    var db = new DataContext();

    Console.WriteLine("Enter the ID of the product to display:");
    if (int.TryParse(Console.ReadLine(), out int productId))
    {
        var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            Console.WriteLine("Product :");
            Console.WriteLine($"ID: {product.ProductId}");
            Console.WriteLine($"Name: {product.ProductName}");
            Console.WriteLine($"Supplier ID: {product.SupplierId}");
            Console.WriteLine($"Category ID: {product.CategoryId}");
            Console.WriteLine($"Quantity Per Unit: {product.QuantityPerUnit}");
            Console.WriteLine($"Unit Price: {product.UnitPrice:C}");
            Console.WriteLine($"Units In Stock: {product.UnitsInStock}");
            Console.WriteLine($"Units On Order: {product.UnitsOnOrder}");
            Console.WriteLine($"Reorder Level: {product.ReorderLevel}");
            Console.WriteLine($"Discontinued: {(product.Discontinued ? "Yes" : "No")}");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }
   
}
/// 


else if (choice == "11")
{
    Console.WriteLine(" Category Name:");
    string categoryName = Console.ReadLine()!;
    Console.WriteLine("Category Description:");
    string? description = Console.ReadLine();

    using (var db = new DataContext())
    {
        var category = new Category
        {
            CategoryName = categoryName,
            Description = description
        };

        db.Categories.Add(category);
        db.SaveChanges();
    }
    Console.WriteLine("Category added ");
    logger.Info($"New category added: {categoryName}");
}
///
/// 
else if (choice == "12")
{
    Console.WriteLine("Enter the ID of the category to edit:");
    if (int.TryParse(Console.ReadLine(), out int categoryId)){
    using (var db = new DataContext())
        {
         var category = db.Categories.Find(categoryId);

      if (category != null)
            {
     Console.WriteLine($"Current Name: {category.CategoryName}");
      Console.WriteLine("Enter new Category Name or press enter:");
       string? newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
    category.CategoryName = newName;

        Console.WriteLine($"Current Description: {category.Description}");
        Console.WriteLine("Enter new Description  or press enter:");
        string? newDescription = Console.ReadLine();
       if (!string.IsNullOrWhiteSpace(newDescription))
          category.Description = newDescription;

        db.SaveChanges();
                Console.WriteLine("Category updated ");
                logger.Info($"Category with ID {categoryId} updated.");
                           }
            else{
                Console.WriteLine(" not found.");
                logger.Info($" {categoryId}, but it does not exist.");
            }
        }
    }
   
}
/// 
else if (choice == "13")
{
    using (var db = new DataContext())
    {
        var categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
        Console.WriteLine("All categories:");
        foreach (var cat in categories)
        {
            Console.WriteLine($"{cat.CategoryName} - {cat.Description}");
        }
    }

    logger.Info("displayed all categories.");
}
/// 
/// 
/// 




/// 
/////
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);
logger.Info("Program ended");





