using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HakimsLivs.Models
{
    public class Order
    {
        public int ID { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required]
        public IList<OrderProduct> OrderProducts { get; set; }
    }
}
