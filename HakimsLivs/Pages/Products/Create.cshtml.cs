using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HakimsLivs.Data;
using HakimsLivs.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HakimsLivs.Pages.Products
{
    public class CreateModel : PageModel
    {
        public List<string> categoryList { get; set; }

        private readonly HakimsLivs.Data.ApplicationDbContext _context;
        private ApplicationDbContext database;

        public CreateModel(HakimsLivs.Data.ApplicationDbContext context, ApplicationDbContext database)
        {
            _context = context;
            this.database = database;
        }

        public async Task OnGet()
        {
            categoryList = await database.Categories.Select(c => c.Name).ToListAsync();;
        }

        [BindProperty]
        public Product Product { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            // Ensures that the image URL given is valid, if not: the "Image not available" shows
            bool exists;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Product.Image);
                request.Method = "HEAD";
                request.GetResponse();
                exists = true;
            }
            catch
            {
                exists = false;
            }

            var newProduct = new Product();
            newProduct.Name = Product.Name;
            newProduct.Price = Product.Price;
            newProduct.Inventory = Product.Inventory;
            newProduct.Weight = Product.Weight;
            newProduct.Volume = Product.Volume;
            if(Product.Image == "" || Product.Image == null || exists == false) { newProduct.Image = @"https://www.feednavigator.com/var/wrbm_gb_food_pharma/storage/images/_aliases/news_large/9/2/8/5/235829-6-eng-GB/Feed-Test-SIC-Feed-20142.jpg"; }
            else if (exists == true) { newProduct.Image = Product.Image; }
            newProduct.Category = database.Categories.Where(c => c.Name == Product.Category.Name).First();


            if (!ModelState.IsValid)
            {
                return Page();
            }

            database.Products.Add(newProduct);
            await database.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
