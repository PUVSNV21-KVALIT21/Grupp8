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

namespace HakimsLivs.Pages.Orders
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public DetailsModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Order> orderList { get; set; } = new List<Order>();

        public List<OrderUserProduct> orderUserProductList { get; set; } = new List<OrderUserProduct>();


        public async Task<IActionResult> OnGetAsync()
        {
            var admin = _context.Users.Where(user => user.UserName == "admin@hakimlivs.se").FirstOrDefault();
            var currentUser = HttpContext.User.Identity.Name;
            if (admin.UserName != currentUser)
            {
                return Redirect("./Identity/Account/AccessDenied?");
            }

            orderList = await _context.Orders.Include(u => u.User).Where(o => o.OrderCompleted == true).OrderBy(o => o.OrderDate).ToListAsync();

            foreach( var order in orderList)
            {
                var productIDList = await _context.OrderProducts.Where(op => op.OrderID == order.ID).Select(op => op.ProductID).ToListAsync();
                var productList =  _context.Products.Where(p => productIDList.Contains(p.ID)).ToList();
                var username = await _context.Users.Where(u => u.Id == order.UserID).Select(u => u.UserName).FirstOrDefaultAsync();
                var productAmountList = new List<ProductAmount>();

                foreach(var product in productList)
                {
                    var amount = _context.OrderProducts.Where(op => op.OrderID == order.ID).Where(op => op.ProductID == product.ID).Count();

                    ProductAmount productAmount = new ProductAmount();
                    productAmount.Product = product;
                    productAmount.Amount = amount;
                    productAmount.TotalPrice = amount * product.Price;
                    productAmountList.Add(productAmount);
                }

                OrderUserProduct orderUserProduct = new OrderUserProduct();
                orderUserProduct.Order = order;
                orderUserProduct.Username = username;
                orderUserProduct.ProductList = productAmountList;
                orderUserProductList.Add(orderUserProduct);

            }
            return Page();
        }

    }
    public class ProductAmount
    {
        public Product Product { get; set; }
        public int Amount { get; set; }
        public decimal TotalPrice { get; set; }

    }

    public class OrderUserProduct
    {
        public Order Order { get; set; }
        public string Username { get; set; }
        public List<ProductAmount> ProductList { get; set; } = new List<ProductAmount>();
    }
}
