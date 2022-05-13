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

namespace HakimsLivs.Pages.Checkout
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public IndexModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<OrderProduct> OrderProduct { get;set; }

        public List<Product> Products { get; set; }
        public Dictionary<Product, int> ProductAmount { get; set; } = new Dictionary<Product, int>();

        public Order Order { get; set; }

        public void OnGet()
        {
            var username = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

            Order = _context.Orders.Where(o => o.User.UserName == username).Where(o => o.OrderCompleted == false).FirstOrDefault();

            var productID = _context.OrderProducts.Where(op => op.OrderID == Order.ID).Select(op => op.ProductID);
            Products = _context.Products.Where(p => productID.Contains(p.ID)).ToList();
            
            foreach (var product in Products)
            {
                var amount = _context.OrderProducts.Where(op => op.OrderID == Order.ID).Where(op => op.ProductID == product.ID).Count();
                ProductAmount.Add(product, amount);
            }
        }
    }
}
