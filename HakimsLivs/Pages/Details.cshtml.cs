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
    public class DetailsModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public DetailsModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
