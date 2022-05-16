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

namespace HakimsLivs.Pages.Categories
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public IndexModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var admin = _context.Users.Where(user => user.UserName == "admin@hakimslivs.se").FirstOrDefault();
            var username = HttpContext.User.Identity.Name;
            if (admin.UserName != username)
            {
                return Redirect("./Identity/Account/AccessDenied?");
            }

            Category = await _context.Categories.ToListAsync();
            return Page();
        }
    }
}
