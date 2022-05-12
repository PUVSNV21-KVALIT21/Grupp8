using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HakimsLivs.Data;
using HakimsLivs.Models;

namespace HakimsLivs.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public DeleteModel(HakimsLivs.Data.ApplicationDbContext context)
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

            OrderProduct = await _context.OrderProducts
                .Include(o => o.Order)
                .Include(o => o.Product).FirstOrDefaultAsync(m => m.ID == id);

            if (OrderProduct == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderProduct = await _context.OrderProducts.FindAsync(id);

            if (OrderProduct != null)
            {
                _context.OrderProducts.Remove(OrderProduct);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
