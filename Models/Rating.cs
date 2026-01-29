using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Website.Models
{
    [Table("Rating")]
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Star { get; set; }
    }
}
