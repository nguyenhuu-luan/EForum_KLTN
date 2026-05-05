using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.Controllers
{
    [Authorize(Roles = "NhanVien,Admin")]
    public class NhanVienController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
