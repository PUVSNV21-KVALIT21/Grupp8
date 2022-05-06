using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HakimsLivs.Data;
using HakimsLivs.Models;
using System.Net;

namespace HakimsLivs.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public EditModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }
        public List<SelectListItem> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            Categories = await _context.Categories.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
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

            var productToupdate = await _context.Products.FindAsync(id);

            if (productToupdate == null)
            {
                return NotFound();
            }

            productToupdate.Name = Product.Name;
            productToupdate.Price = Product.Price;
            productToupdate.Weight = Product.Weight;
            productToupdate.Volume = Product.Volume;
            productToupdate.Inventory = Product.Inventory;
            if (Product.Image == "" || Product.Image == null || exists == false) { productToupdate.Image = @"https://www.feednavigator.com/var/wrbm_gb_food_pharma/storage/images/_aliases/news_large/9/2/8/5/235829-6-eng-GB/Feed-Test-SIC-Feed-20142.jpg"; }
            else if (exists == true) { productToupdate.Image = Product.Image; }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", productToupdate);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}