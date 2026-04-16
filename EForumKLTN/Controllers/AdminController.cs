using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EForumKLTN.Models;
using System.Linq;

[Authorize(Roles = "Admin")] // Khóa cửa: Chỉ Admin mới vào được
public class AdminController : Controller
{
    private readonly EForumContext _db;

    public AdminController(EForumContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        // Lấy vài con số thống kê cho chuyên nghiệp
        ViewBag.TotalUsers = _db.KhachHangs.Count();
        ViewBag.TotalPosts = _db.BaiViets.Count();
        ViewBag.TotalEbooks = _db.HangHoas.Count();

        return View();
    }
}