using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies; 
using EForumKLTN.Models;
using System.Linq;

[Authorize(Roles = "Admin")] 
public class AdminController : Controller
{
    private readonly EForumContext _db;

    public AdminController(EForumContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        ViewBag.TotalUsers = _db.KhachHangs.Count();
        ViewBag.TotalPosts = _db.BaiViets.Count();
        ViewBag.TotalEbooks = _db.HangHoas.Count();

        return View();
    }

}