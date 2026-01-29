using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models
{
    [Table("News")]
    public class News
    {
        // Id - Kiểu int, thường là khóa chính
        public int Id { get; set; }

        // Name - Kiểu string (tương đương nvarchar(500))
        public string Name { get; set; }

        // Description - Kiểu string (tương đương ntext/nvarchar(max))
        public string Description { get; set; }

        // Content - Kiểu string (tương đương ntext/nvarchar(max))
        // Đã đổi tên thuộc tính từ [Content] sang Content để tuân theo quy tắc đặt tên chuẩn C#
        public string Content { get; set; }

        // Hot - Kiểu int
        public int Hot { get; set; }

        // Photo - Kiểu string (tương đương nvarchar(500)), dùng để lưu đường dẫn ảnh
        public string Photo { get; set; }
    }
}