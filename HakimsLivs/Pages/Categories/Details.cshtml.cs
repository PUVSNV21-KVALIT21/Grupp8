using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HakimsLivs.Data;
using HakimsLivs.Models;

namespace HakimsLivs.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private ApplicationDbContext database;

        public DetailsModel(HakimsLivs.Data.ApplicationDbContext database)
        {
            this.database = database;
        }
        public List<Microsoft.AspNetCore.Identity.IdentityUser> Customer { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var admin = database.Users.Where(user => user.UserName == "admin@hakimslivs.se").FirstOrDefault();
            var username = HttpContext.User.Identity.Name;
            if (admin.UserName != username)
            {
                return Redirect("./Identity/Account/AccessDenied?");
            }

            Customer = await database.Users.ToListAsync();

            return Page();
        }
    }
}
