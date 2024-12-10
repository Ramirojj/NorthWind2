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
Console.WriteLine("8) Edit a specified record from the Products table");
Console.WriteLine("9) Display a specific Product (all product fields should be displayed)");
Console.WriteLine("10) Use NLog to track user functions");
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



/////
///
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