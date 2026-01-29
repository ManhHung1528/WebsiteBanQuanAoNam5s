using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Website.Models
{
    [Table("Categories")]

    public class RowCategory
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int? DisplayHomePage { get; set; }
    }
}
