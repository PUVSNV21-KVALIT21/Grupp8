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
    public class IndexModel : PageModel
    {
        private readonly HakimsLivs.Data.ApplicationDbContext _context;

        public IndexModel(HakimsLivs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }
        public List<ProductsInOrder> ProductsInOrderList { get; set; } = new();
        public IList<Product> Products { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public Product Product { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User == user).FirstOrDefaultAsync();

            OrderProducts = await _context.OrderProducts.Where(o => o.OrderID == Order.ID).ToListAsync();

            foreach (var product in OrderProducts)
            {
                ProductsInOrder products = new ProductsInOrder();

                int productID = product.ProductID;
                Product = await _context.Products.Where(o => o.ID == productID).FirstOrDefaultAsync();
                int amount = 1;

                products.ProductName = Product.Name;
                products.ProductID = Product.ID;
                products.Amount = amount;
                products.PricePerItem = Product.Price;

                if (!ProductsInOrderList.Any(item => item.ProductID == productID))
                {
                    products.TotalItemPrice = products.Amount * products.PricePerItem;
                    ProductsInOrderList.Add(products);
                }
                else
                {
                    ProductsInOrder productToChange = ProductsInOrderList.Where(p => p.ProductID == productID).FirstOrDefault();
                    ProductsInOrderList.Remove(productToChange);
                    int amountToChange = productToChange.Amount;
                    amountToChange++;
                    productToChange.Amount = amountToChange;
                    productToChange.TotalItemPrice = productToChange.Amount * productToChange.PricePerItem;

                    ProductsInOrderList.Add(productToChange);
                }
            }
            ProductsInOrderList = ProductsInOrderList.OrderBy(p => p.ProductName).ToList();
            return Page();
        }

        public async Task<IActionResult> RemoveAsync()
        {
            // TODO: Skriv kod för att ta bort ur varukorgen


            return Redirect("./Index");
        }

        public async Task<IActionResult> OnPostIncreaseAsync()
        {
            var username = HttpContext.User.Identity.Name;
            //var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();
            
            var selectedProductID = int.Parse(Request.Form.Keys.First());

            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User.UserName == username).FirstOrDefaultAsync();

           var newOrderProduct = new OrderProduct();
            newOrderProduct.ProductID = selectedProductID;
            newOrderProduct.OrderID = Order.ID;
            _context.OrderProducts.Add(newOrderProduct);
            _context.SaveChanges();
            
            return Redirect("./Orders");
        }

        public async Task<IActionResult> OnPostDecreaseAsync()
        {
            var username = HttpContext.User.Identity.Name;
            //var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

            var selectedProductID = int.Parse(Request.Form.Keys.First());

            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User.UserName == username).FirstOrDefaultAsync();

            var orderProductdecrease = _context.OrderProducts.Where(op => op.ProductID == selectedProductID).Where(op => op.OrderID == Order.ID).FirstOrDefault();
            _context.OrderProducts.Remove(orderProductdecrease);
            _context.SaveChanges();

            return Redirect("./Orders");
        }
    }

    public class ProductsInOrder
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal TotalItemPrice { get; set; }
    }

}