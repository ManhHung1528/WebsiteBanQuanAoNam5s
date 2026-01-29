
// Phải thêm mới kế thừa class Attributes 
// Khi muốn tạo ra 1 flie Attribute do người dùng định nghĩa thì phải add thư viện ấy
using Microsoft.AspNetCore.Mvc.Filters;
namespace Website.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra xem session Admin_User_Email đã tồn tại chưa
            // (Nếu tồn tại user login thành công, nếu chưa chuyến đến login để User đăng nhập)
            if (String.IsNullOrEmpty(context.HttpContext.Session.GetString("Admin_Email")))
                context.HttpContext.Response.Redirect("Admin/Account/Login");
            base.OnActionExecuting(context);
        }
    }
}

