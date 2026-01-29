using Microsoft.AspNetCore.Http;
//muon su dung thu vien jSon thi phai them dong duoi
using Newtonsoft.Json;
using Website.Models;

namespace Website.Models
{
    public class Cart
    {
        protected static readonly MyDbContext db = new MyDbContext();
        //------        
        public static T GetObjectFromJson<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        //------
        //lay gio hang dang ton tai
        public static List<Item> GetCart(ISession session)
        {
            //JsonConvert.DeserializeObject<T>("cart")
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            return cart;
        }
        //add item to cart
        public static void CartAdd(ISession session, int productId, int sizeId, int colorId)
        {
            // lấy giỏ hàng hiện tại
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart") ?? new List<Item>();

            // kiểm tra sản phẩm cùng size và color đã tồn tại chưa
            var existingItem = cart.FirstOrDefault(i =>
                i.ProductRecord.Id == productId &&
                i.SizeId == sizeId &&
                i.ColorId == colorId);

            if (existingItem != null)
            {
                // nếu đã có thì tăng số lượng
                existingItem.Quantity++;
            }
            else
            {
                // nếu chưa có thì thêm mới
                Products product = db.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    cart.Add(new Item
                    {
                        ProductRecord = product,
                        Quantity = 1,
                        SizeId = sizeId,
                        ColorId = colorId
                    });
                }
            }

            // lưu lại giỏ hàng vào session
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }

        //remove item in cart
        public static void CartRemove(ISession session, int id)
        {
            //convert chuoi json thanh List<Item>
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            //lay vi tri cua phan tu muon xoa bang cach goi ham isExists -> tra ve vi tri cua phan tu muon xoa
            int index = isExist(session, id);
            //goi ham xoa phan tu trong gio hang
            cart.RemoveAt(index);
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        //remove all item in cart
        public static void CartDestroy(ISession session)
        {
            //xoa toan bo gio hang <=> khoi tao lai gio hang
            List<Item> cart = new List<Item>();
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        public static void CartUpdate(ISession session, int id, int quantity)
        {
            //convert chuoi json thanh List<Item>
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            //---
            //duyet tung phan tu cua gio hang, sau do update lai so luong tuong ung voi id, quantity truyen vao
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    cart[i].Quantity = quantity;
                }
            }
            //---
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        private static int isExist(ISession session, int id)
        {
            //convert chuoi json thanh List<Item>
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            //duyet tung phan tu cua session array, neu phan tu nao co id trung voi id truyen vao thi se tra ve chi so cua phan tu do (chi so chay tu 0 den n)
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public static double CartTotal(ISession session)
        {
            List<Item> items_cart = Cart.GetCart(session);
            if (items_cart != null)
            {
                double total = 0;
                foreach (var item in items_cart)
                {
                    if (item?.ProductRecord == null) continue;
                    //total += item.Quantity * (item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100);
                    total += item.Quantity * (item.ProductRecord.Price - item.ProductRecord.Discount);
                }
                return total;
            }
            else
                return 0;
        }
        public static void CartCheckOut(ISession session, int customer_id)
        {
            //khởi tạo đối tượng thao tác csdl
            MyDbContext db = new MyDbContext();
            //---
            List<Item> _cart = Cart.GetCart(session);
            //insert du lieu vao table Orders
            Order _RecordOrder = new Order();
            _RecordOrder.CustomerId = customer_id;
            _RecordOrder.Create = DateTime.Now;
            _RecordOrder.Price = _cart.Sum(tbl => tbl.ProductRecord.Price * tbl.Quantity);
            db.Order.Add(_RecordOrder);
            db.SaveChanges();
            //lay id vua insert
            int order_id = _RecordOrder.Id;
            //duyet cac san pham trong session, moi phan tu se add thanh 1 ban ghi trong OrdersDetail
            foreach (var item in _cart)
            {
                OrderDetail _RecordOrdersDetail = new OrderDetail();
                _RecordOrdersDetail.OrderId = order_id;
                _RecordOrdersDetail.ProductId = item.ProductRecord.Id;
                _RecordOrdersDetail.Price = item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100;
                _RecordOrdersDetail.Quantity = item.Quantity;
                //---
                db.OrdersDetails.Add(_RecordOrdersDetail);
                db.SaveChanges();
            }
            //xoa tat cac cac phan tu trong gio hang
            Cart.CartDestroy(session);
        }
        //lấy số sản phẩm trong giỏ hàng
        public static int CartQuantity(ISession session)
        {
            List<Item> items_cart = Cart.GetCart(session);
            if (items_cart != null)
            {
                return items_cart.Count;
            }
            else
                return 0;
        }
    }
}
