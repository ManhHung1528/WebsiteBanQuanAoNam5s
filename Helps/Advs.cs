using Website.Models;
using Website.Models;

namespace Website.Helper
{

    public class Advs
    {
        public static MyDbContext db = new MyDbContext();
        public static List<Adv> GetAdv(int _position)
        {
            List<Adv> items = db.Advs.Where(item => item.Position == _position).OrderByDescending(item => item.Id).ToList();
            return items != null ? items : new List<Adv>();
        }
    }
}
