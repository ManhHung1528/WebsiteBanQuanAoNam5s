using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models
{
    [Table("CategoriesProducts")]
    public class CategoriesProduct
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
    }
}
