using HakimsLivs.Data;
using HakimsLivs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HakimsLivs.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext database;
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext database, HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            this.database = database;
        }

        public List<string> categoriesInProduct { get; set; }
        public IList<Product> ProductList { get; set; }

        public int ItemsInOrder { get; set; } = 0;

        [BindProperty]
        public Order Order { get; set; }

        public bool categoryIsSelected { get; set; } = false;
        public bool admin { get; set; } = false;

        public void OnGet()
        {
            if(HttpContext.User.Identity.Name == "admin@hakimslivs.se")
            {
                admin = true;
            }

            #region If database is empty => products are loaded from CSV files
            var categorieExist = database.Categories.Any();
            var productsExist = database.Products.Any();
            if (categorieExist == false || productsExist == false)
            {
                string[] categories = System.IO.File.ReadAllLines(@"Data\HLCategories.csv", System.Text.Encoding.GetEncoding("ISO-8859-1"));
                foreach (string name in categories)
                {
                    Category category = new Category
                    {
                        Name = name,
                    };
                    database.Categories.Add(category);
                    database.SaveChanges();
                }

                string[] products = System.IO.File.ReadAllLines(@"Data\HLProducts.csv", System.Text.Encoding.GetEncoding("ISO-8859-1"));
                foreach (string entry in products)
                {
                    string[] split = entry.Split(';');
                    string name = split[0];
                    string price = split[1];
                    string gram = split[2];
                    string mililiter = split[3];
                    string amount = split[4];
                    string picture = split[5];
                    string categoryName = split[6];
                    var category = database.Categories.Single(m => m.Name == categoryName);
                    if (split[2] == "")
                    {
                        Product product = new Product
                        {
                            Name = name,
                            Price = decimal.Parse(price),
                            Weight = null,
                            Volume = int.Parse(mililiter),
                            Category = category,
                            Inventory = int.Parse(amount),
                            Image = picture
                        };
                        database.Products.Add(product);
                    }
                    if (split[3] == "")
                    {
                        Product product = new Product
                        {
                            Name = name,
                            Price = decimal.Parse(price),
                            Weight = int.Parse(gram),
                            Volume = null,
                            Category = category,
                            Inventory = int.Parse(amount),
                            Image = picture
                        };
                        database.Products.Add(product);
                    }
                    database.SaveChanges();                
                }
            }
            #endregion

            var Categories = database.Products.Where(p => p.Inventory > 0).Select(p => p.Category).AsEnumerable().GroupBy(c => c.Name).ToList();
            categoriesInProduct = Categories.Select(c => c.Key).ToList();
            if (categoryIsSelected == false) {
                ProductList = database.Products.ToList();
            }

            if (HttpContext.User.Identity.Name != null)
            {
                try 
                {
                    var currentOrder = database.Orders.Where(o => o.User.UserName == HttpContext.User.Identity.Name).Where(o => o.OrderCompleted == false).FirstOrDefault();
                    List<Product> productlist = new List<Product>();
                    var opList = database.OrderProducts.Where(op => op.OrderID == currentOrder.ID).Select(op => op.ProductID).ToList();
                    foreach( var op in opList)
                    {
                        var p = database.Products.Where(p => p.Archived == false).Where(p => p.ID == op).FirstOrDefault();
                        productlist.Add(p);
                    }
                    ItemsInOrder = productlist.Where(entry => entry != null).Count();
                }
                catch { }
            }
        }

        public void OnPost()
        {
            if (HttpContext.User.Identity.Name == "admin@hakimslivs.se")
            {
                admin = true;
            }

            var Categories = database.Products.Where(p => p.Inventory > 0).Select(p => p.Category).AsEnumerable().GroupBy(c => c.Name).ToList();
            categoriesInProduct = Categories.Select(c => c.Key).ToList();

            categoryIsSelected = true;
            var selectedCategory = Request.Form.Keys.First();
            if(selectedCategory == "All")
            {
                ProductList = database.Products.ToList();
            }
            else
            {
                ProductList = database.Products.Where(c => c.Category.Name == selectedCategory).ToList();
            }

            if (HttpContext.User.Identity.Name != null)
            {
                try
                {
                    var currentOrder = database.Orders.Where(o => o.User.UserName == HttpContext.User.Identity.Name).Where(o => o.OrderCompleted == false).FirstOrDefault();
                    var opList = database.OrderProducts.Where(op => op.OrderID == currentOrder.ID).Select(op => op.ProductID).ToList();
                    List<Product> productlist = new List<Product>();
                    foreach (var op in opList)
                    {
                        var p = database.Products.Where(p => p.Archived == false).Where(p => p.ID == op).FirstOrDefault();
                        productlist.Add(p);
                    }
                    ItemsInOrder = productlist.Count();
                }
                catch { }
            }

            Page();
        }
        
        public async Task<IActionResult> OnPostViewAsync()
        {
            if (HttpContext.User.Identity.Name == "admin@hakimslivs.se")
            {
                admin = true;
            }

            #region //categoriesInProduct & ProductList needs to be defined when page reloads
            var Categories = database.Products.Where(p => p.Inventory > 0).Select(p => p.Category).AsEnumerable().GroupBy(c => c.Name).ToList();
            categoriesInProduct = Categories.Select(c => c.Key).ToList();
            ProductList = database.Products.ToList();
            #endregion

            //Check the the username of current user. If not Logged-in username=null
            var username = HttpContext.User.Identity.Name;

            //If the username is null, the user is redirected to the login page
            if(username == null){ return Redirect("./Identity/Account/Login?ReturnUrl=%2FIndex"); }

            //Otherwise, the user if found in the database
            var user = database.Users.Where(u => u.UserName == username).FirstOrDefault();

            //The ProductID is sent with the form that the button lies within
            var selectedProductID = int.Parse(Request.Form.Keys.First());

            //Check if there are any open/ongoing orders
            var currentOrder = database.Orders.Where(o => o.User.UserName == username).Where(o => o.OrderCompleted == false).FirstOrDefault();
            
            if(currentOrder == null) //If not, and order is created, and products added to the OrderProduct class
            {
                var newOrder = new Order();
                newOrder.User = user;
                newOrder.OrderDate = DateTime.Now;
                newOrder.OrderCompleted = false;
                database.Orders.Add(newOrder);
                database.SaveChanges();

                var newOrderProduct = new OrderProduct();
                newOrderProduct.ProductID = selectedProductID;
                newOrderProduct.OrderID = newOrder.ID;
                database.OrderProducts.Add(newOrderProduct);
                database.SaveChanges();

                ItemsInOrder = database.OrderProducts.Where(op => op.OrderID == newOrderProduct.ID).Count();
            }
            else //If there is already and ongoing order, the product is added to it. 
            {
                var newOrderProduct = new OrderProduct();
                newOrderProduct.ProductID = selectedProductID;
                newOrderProduct.OrderID = currentOrder.ID;
                database.OrderProducts.Add(newOrderProduct);
                database.SaveChanges();

                ItemsInOrder = database.OrderProducts.Where(op => op.OrderID == currentOrder.ID).Count();
            }
            return Page();
        }
    }
}
