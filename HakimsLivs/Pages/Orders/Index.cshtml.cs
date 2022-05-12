using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HakimsLivs.Data;
using HakimsLivs.Models;

namespace HakimsLivs.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public IndexModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<OrderProduct> OrderProduct { get;set; }

        public async Task OnGetAsync()
        {
            OrderProduct = await _context.OrderProducts
                .Include(o => o.Order).Where()
                .Include(o => o.Product).ToListAsync();
        }
    }
}
