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

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext database)
        {
            _logger = logger;
            this.database = database;
        }


        public void OnGet()
        {
            var categorieExist = database.Categories.Any();
            var productsExist = database.Products.Any();
            if (categorieExist == false || productsExist == false)
            {
                string[] categories = System.IO.File.ReadAllLines(@"Data\HLCategories.csv");
                foreach (string name in categories)
                {
                    Category category = new Category
                    {
                        Name = name,
                    };
                    database.Categories.Add(category);
                    database.SaveChanges();
                }

                string[] products = System.IO.File.ReadAllLines(@"Data\HLProducts.csv");
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

        }
    }
}
