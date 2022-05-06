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

namespace HakimsLivs.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;
        private ApplicationDbContext database;

        public CreateModel(HakimsLivs.Data.ApplicationDbContext context, ApplicationDbContext database)
        {
            _context = context;
            this.database = database;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            List<string> categoryList = database.Categories.Select(c => c.Name).ToList();
            if (!ModelState.IsValid || categoryList.Contains(Category.Name))
            {
                return Page();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
