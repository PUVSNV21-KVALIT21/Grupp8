using HakimsLivs.Data;
using HakimsLivs.Models;
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

        public bool categoryIsSelected { get; set; } = false;
        //[BindProperty]
        //public string searchString { get; set; }

        public void OnGet()
        {
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

            //Categories =  database.Categories.OrderBy(c => c.Name).Select(c => c.Name).ToList();
            var Categories = database.Products.Where(p => p.Inventory > 0).Select(p => p.Category).AsEnumerable().GroupBy(c => c.Name).ToList();
            categoriesInProduct = Categories.Select(c => c.Key).ToList();
            if (categoryIsSelected == false) {
                ProductList = database.Products.ToList();
            }

            

            var saraLevin = database.Users.Where(user => user.UserName == "sara.levin96@gmail.com");
            var test2 = database.UserRoles.ToList();
            var test3 = database.Roles.ToList();

           

            var test = database.Users.Select(user => user.UserName).ToList();


        }
        public void OnPost()
        {
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

            //if (!String.IsNullOrEmpty(searchString)) 
            //{ 
            //    ProductList = ProductList.Where(s => s.Name.ToLower().Contains(searchString)).ToList(); 
            //}

            Page();
        }
    }
}
