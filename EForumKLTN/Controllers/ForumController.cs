using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EForumKLTN.Models;

public class ForumController : Controller
{
    private readonly EForumContext _db;

    public ForumController(EForumContext db)
    {
        _db = db;
    }

    // 1. Trang chủ Forum: Liệt kê các Chủ đề (MaCD)
    public async Task<IActionResult> Index()
    {
        var chuDes = await _db.ChuDes.ToListAsync();
        return View(chuDes);
    }

    // 2. Danh sách bài viết trong một Chủ đề
    public async Task<IActionResult> Topics(int id)
    {
        var chuDe = await _db.ChuDes
            .Include(c => c.BaiViets) 
            .ThenInclude(b => b.MaKhNavigation) 
            .FirstOrDefaultAsync(c => c.MaCd == id);

        if (chuDe == null) return NotFound();

        return View(chuDe);
    }

    // 1. Trang hiển thị Form đăng bài
    [HttpGet]
    public IActionResult Create(int? maCd)
    {
        // Kiểm tra đăng nhập (Giả sử ông lưu MaKh vào Session khi Login)
        if (HttpContext.Session.GetString("MaKh") == null)
        {
            return RedirectToAction("DangNhap", "KhachHang");
        }

        ViewBag.MaCd = maCd; // Để mặc định chọn đúng chủ đề đang đứng
        return View();
    }

    // 2. Xử lý lưu bài viết
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BaiViet model)
    {
        var maKh = HttpContext.Session.GetString("MaKh");
        if (maKh == null) return RedirectToAction("DangNhap", "KhachHang");

        if (ModelState.IsValid)
        {
            model.MaKh = maKh;
            model.NgayDang = DateTime.Now;

            _db.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Topics", new { id = model.MaCd });
        }
        return View(model);
    }
}