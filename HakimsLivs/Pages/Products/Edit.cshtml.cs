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
            productToupdate.Image = Product.Image;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", productToupdate);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
