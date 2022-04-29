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
        // TODO: Denna ska ändras när modellen för kategori har skapats!
        [Required(ErrorMessage = "Välj en kategori.")]
        public Category Category { get; set; }
        [Required]
        public int Inventory { get; set; } = 0;
        public string Image { get; set; }
    }
}
