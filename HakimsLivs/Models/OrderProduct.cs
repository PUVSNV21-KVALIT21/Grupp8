using System.ComponentModel.DataAnnotations;

namespace HakimsLivs.Models
{
    public class OrderProduct
    {
        public int ID { get; set; }
        [Required]
        public int OrderID { get; set; }
        [Required]
        public Order Order { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public Product Product { get; set; }
    }
}
