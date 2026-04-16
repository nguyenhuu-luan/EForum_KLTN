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
}