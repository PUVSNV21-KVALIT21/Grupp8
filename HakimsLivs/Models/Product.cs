using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HakimsLivs.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Produkten måste ha ett namn.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Produkten måste ha ett pris.")]
        [Column (TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        public int? Weight { get; set; }
        public int? Volume { get; set; }
        [Required(ErrorMessage = "Välj en kategori.")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        [Required]
        public int Inventory { get; set; } = 0;
        public string Image { get; set; } // TODO: Här ska pekas till en placeholderbild
        public IList<OrderProduct> OrderProduct { get; set; }
    }
}
