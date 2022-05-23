using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HakimsLivs.Models
{
    public class Order
    {
        public int ID { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public bool OrderCompleted { get; set; } = false; 
        public IdentityUser User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        //[Required]
        //public List<Product> Products { get; set; } = new List<Product>();
    }
}
