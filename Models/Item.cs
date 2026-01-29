namespace Website.Models
{
    public class Item
    {
        //thong tin san pham
        public Products ProductRecord { get; set; }
        //so luong
        public int Quantity { get; set; }
        public int SizeId { get; set; } 
        public int ColorId { get; set; }
        public string SizeName { get; set; }
        public string ColorName { get; set; }
    }
}
