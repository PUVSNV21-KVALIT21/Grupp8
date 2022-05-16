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

namespace HakimsLivs.Pages.Checkout
{
    public class EditModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public EditModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrderProduct OrderProduct { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderProduct = await _context.OrderProducts.FirstOrDefaultAsync(m => m.ID == id);

            if (OrderProduct == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(OrderProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderProductExists(OrderProduct.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool OrderProductExists(int id)
        {
            return _context.OrderProducts.Any(e => e.ID == id);
        }
    }
}
