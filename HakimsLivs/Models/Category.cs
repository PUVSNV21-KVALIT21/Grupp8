using System.ComponentModel.DataAnnotations;

namespace HakimsLivs.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Kategorin måste ha ett namn")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
