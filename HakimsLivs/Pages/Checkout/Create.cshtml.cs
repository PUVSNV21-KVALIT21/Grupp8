using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HakimsLivs.Data;
using HakimsLivs.Models;

namespace HakimsLivs.Pages.Checkout
{
    public class CreateModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public CreateModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public OrderProduct OrderProduct { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.OrderProducts.Add(OrderProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
