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
        public decimal TotalPrice { get; set; } = 0;


        public async Task<IActionResult> OnGetAsync()
        {
            // Getting the user that owns the cart
            var username = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

            // Getting the current active order for the user
            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User == user).FirstOrDefaultAsync();

            // Getting all the products linked to the order.
            OrderProducts = await _context.OrderProducts.Where(o => o.OrderID == Order.ID).ToListAsync();

            foreach (var product in OrderProducts)
            {
                ProductsInOrder products = new ProductsInOrder();

                // Setting some variables used in displaying the cart to the user
                int productID = product.ProductID;
                Product = await _context.Products.Where(o => o.ID == productID).FirstOrDefaultAsync();
                int amount = 1;

                // Putting all the data needed into the cart list
                products.ProductName = Product.Name;
                products.ProductID = Product.ID;
                products.Amount = amount;
                products.PricePerItem = Product.Price;

                // If the product is not already in the cart it will be added
                if (!ProductsInOrderList.Any(item => item.ProductID == productID))
                {
                    products.TotalItemPrice = products.Amount * products.PricePerItem;
                    ProductsInOrderList.Add(products);
                }
                // If the product already exists the amount in the cart will be increased
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

            // Genereating the total price for the entire cart
            foreach (ProductsInOrder p in ProductsInOrderList)
            {
                TotalPrice += p.PricePerItem * p.Amount;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync()
        {
            var selectedProductID = int.Parse(Request.Form.Keys.First());

            // Looping through the table in db to remove everey line containing the product chosen
            foreach (OrderProduct item in _context.OrderProducts)
            {
                if (item.ProductID == selectedProductID)
                {
                    _context.OrderProducts.Remove(item);
                }
            }
            await _context.SaveChangesAsync();
            return Redirect("./Orders");
        }

        public async Task<IActionResult> OnPostIncreaseAsync()
        {
            var username = HttpContext.User.Identity.Name;

            var selectedProductID = int.Parse(Request.Form.Keys.First());

            // Finding the first line in the table where the chosen product is
            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User.UserName == username).FirstOrDefaultAsync();

            //Creating a new object of the chosen product and adding it to the database
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

            var selectedProductID = int.Parse(Request.Form.Keys.First());

            // Finding the first line in the table where the cosen product is
            Order = await _context.Orders.Where(o => o.OrderCompleted == false).Where(o => o.User.UserName == username).FirstOrDefaultAsync();

            OrderProducts = await _context.OrderProducts.Where(o => o.OrderID == Order.ID).Where(o => o.ProductID == selectedProductID).ToListAsync();

            if (OrderProducts.Count > 1)
            {
                // Removing the line containg the chosen product from the db
                var orderProductdecrease = _context.OrderProducts.Where(op => op.ProductID == selectedProductID).Where(op => op.OrderID == Order.ID).FirstOrDefault();
                _context.OrderProducts.Remove(orderProductdecrease);
                _context.SaveChanges();
            }         

            return Redirect("./Orders");
        }
    }

    // The class for handling the objects in the cart
    public class ProductsInOrder
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal TotalItemPrice { get; set; }
    }
}