using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HakimsLivs.Data;
using HakimsLivs.Models;
using Microsoft.AspNetCore.Authorization;

namespace HakimsLivs.Pages.Products
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public DeleteModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var admin = _context.Users.Where(user => user.UserName == "admin@hakimlivs.se").FirstOrDefault();
            var username = HttpContext.User.Identity.Name;
            if (admin.UserName != username)
            {
                return Redirect("./Identity/Account/AccessDenied?");
            }

            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products.FirstOrDefaultAsync(m => m.ID == id);

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var admin = _context.Users.Where(user => user.UserName == "admin@hakimlivs.se").FirstOrDefault();
            var username = HttpContext.User.Identity.Name;
            if (admin.UserName != username)
            {
                return Redirect("./Identity/Account/AccessDenied?");
            }

            if (id == null)
            {
                return NotFound();
            }

            var productToArchive = await _context.Products.FindAsync(id);

            if (Product != null)
            {
                if (productToArchive.Archived)
                {
                    productToArchive.Archived = false;
                }
                else
                {
                    productToArchive.Archived = true;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
