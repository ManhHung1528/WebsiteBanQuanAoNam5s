using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models
{
    [Table("Adv")] // Đặt tên bảng cụ thể trong Database
    public class Adv
    {
        // Id - Kiểu int
        // Thuộc tính [Key] đánh dấu đây là Khóa chính.
        // Unchecked: Thường có nghĩa là cột này không cho phép NULL và là cột IDENTITY (tự tăng)
        [Key]
        public int Id { get; set; }

        // Name - Kiểu nvarchar(500)
        // Checked: Cho phép NULL, nên sử dụng dấu '?' cho kiểu string trong C# là không cần thiết
        // Nhưng nếu là kiểu giá trị (int, double...) thì cần thêm '?' nếu Checked=True.
        public string? Name { get; set; }

        // Photo - Kiểu nvarchar(500)
        // Checked: Cho phép NULL
        public string? Photo { get; set; }

        // Position - Kiểu int
        // Checked: Cho phép NULL (sử dụng int? - Nullable int)
        public int? Position { get; set; }
    }
}