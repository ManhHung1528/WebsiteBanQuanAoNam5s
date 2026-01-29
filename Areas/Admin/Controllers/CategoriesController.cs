using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.Attributes;
using Website.Areas.Admin.Helper;//để nhìn thấy file MyClass.cs
using Website.Models;
using System.Data;//Sử dụng DataTable, DataSet
using System.Data.SqlClient;//Sử dụng Connection, SqlDataAdapter, SqlCommand,...
//sử dụng thư viện sau để phân trang
using X.PagedList;

namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class CategoriesController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int? page)
        {
            string strConnectionString = MyClass.GetConnectionString();
            DataTable dtCategories = new DataTable();
            //tạo List để đổ dữ liệu vào, chuẩn bị phân trang
            List<RowCategory> listCategories = new List<RowCategory>();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where ParentId = 0 order by Id desc", conn);
                da.Fill(dtCategories);
                //đổ dữ liệu từ datatable vào list
                if (dtCategories.Rows.Count > 0)
                {
                    foreach (DataRow row in dtCategories.Rows)
                    {
                        listCategories.Add(new RowCategory() { Id = Convert.ToInt32(row["Id"]), ParentId = Convert.ToInt32(row["ParentId"]), Name = row["Name"].ToString(), DisplayHomePage = Convert.ToInt32(row["DisplayHomePage"]) });
                    }
                }
            }
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 10;
            return View("Read", listCategories.ToPagedList(page_number, page_size));
        }
        public IActionResult Update(int id)
        {
            //lấy 1 bản ghi tương ứng với id truyền vào
            RowCategory rowCategory = db.Categories.FirstOrDefault(item => item.Id == id);
            //Tạo biến formAction để lưu action của form
            ViewBag.formAction = "/Admin/Categories/UpdatePost/" + id;
            return View("CreateUpdate", rowCategory);
        }
        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            string _Name = fc["Name"];
            int _DisplayHomePage = !String.IsNullOrEmpty(fc["DisplayHomePage"]) ? 1 : 0;
            int _ParentId = Convert.ToInt32(fc["ParentId"]);
            //sử dụng ADO
            string strConnectionString = MyClass.GetConnectionString();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //insert, update, delete thì phải mở kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name,ParentId=@var_parent_id, DisplayHomePage = @display_home_page where Id=@var_id", conn);
                cmd.Parameters.AddWithValue("@var_name", _Name);
                cmd.Parameters.AddWithValue("@var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("@display_home_page", _DisplayHomePage);
                cmd.Parameters.AddWithValue("@var_id", id);
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        //Create
        public IActionResult Create()
        {
            ViewBag.formAction = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }
        //CreatePost
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"];
            int _DisplayHomePage = !String.IsNullOrEmpty(fc["DisplayHomePage"]) ? 1 : 0;
            int _ParentId = Convert.ToInt32(fc["ParentId"]);
            //sử dụng ADO
            string strConnectionString = MyClass.GetConnectionString();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //insert, update, delete thì phải mở kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Categories(Name,ParentId,DisplayHomePage) values(@var_name,@var_parent_id,@display_home_page)", conn);
                cmd.Parameters.AddWithValue("@var_name", _Name);
                cmd.Parameters.AddWithValue("@var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("@display_home_page", _DisplayHomePage);
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        //Delete
        public IActionResult Delete(int id)
        {
            //sử dụng ADO
            string strConnectionString = MyClass.GetConnectionString();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //insert, update, delete thì phải mở kết nối
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from Categories where Id=@var_id or ParentId = @var_id", conn);
                cmd.Parameters.AddWithValue("@var_id", id);
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
    }
}
