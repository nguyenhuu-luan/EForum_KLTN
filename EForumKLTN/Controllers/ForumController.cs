using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EForumKLTN.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ForumController : Controller
{
    private readonly EForumContext _db;

    public ForumController(EForumContext db)
    {
        _db = db;
    }

    #region homeforum
    public async Task<IActionResult> Index()
    {
        var chuDes = await _db.ChuDes.ToListAsync();
        return View(chuDes);
    }
    #endregion

    #region cateforum
    public async Task<IActionResult> Topics(int id)
    {
        var chuDe = await _db.ChuDes
            .Include(c => c.BaiViets)
            .ThenInclude(b => b.MaKhNavigation)
            .FirstOrDefaultAsync(c => c.MaCd == id);

        if (chuDe == null) return NotFound();

        return View(chuDe);
    }
    #endregion

    #region posttopic
    [HttpGet]
    [Authorize] // Tự động đá về Login + kèm ReturnUrl nếu chưa login
    public IActionResult Create(int? maCd)
    {
        // Dùng SelectList để View đổ Dropdown cho dễ
        ViewBag.ChuDes = new SelectList(_db.ChuDes.ToList(), "MaCd", "TenChuDe");
        ViewBag.MaCd = maCd;
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BaiViet model)
    {
        if (ModelState.IsValid)
        {
            model.MaKh = User.Claims.FirstOrDefault(c => c.Type == "CustomerID")?.Value;
            model.NgayDang = DateTime.Now;

            _db.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Topics", new { id = model.MaCd });
        }

        ViewBag.ChuDes = new SelectList(_db.ChuDes.ToList(), "MaCd", "TenChuDe");
        return View(model);
    }
    #endregion

    #region topicdetail
    public async Task<IActionResult> Details(int id)
    {
        var baiViet = await _db.BaiViets
            .Include(b => b.MaKhNavigation)
            .Include(b => b.MaCdNavigation)
            .Include(b => b.BinhLuans)
                .ThenInclude(bl => bl.MaKhNavigation)
            .FirstOrDefaultAsync(b => b.MaBv == id);

        if (baiViet == null) return NotFound();

        return View(baiViet);
    }
    #endregion

    #region post_comment
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostComment(int maBv, string noiDung)
    {
        if (string.IsNullOrWhiteSpace(noiDung))
            return RedirectToAction("Details", new { id = maBv });

        var binhLuan = new BinhLuan
        {
            MaBv = maBv,
            NoiDung = noiDung,
            NgayBl = DateTime.Now,
            MaKh = User.Claims.FirstOrDefault(c => c.Type == "CustomerID")?.Value
        };

        _db.BinhLuans.Add(binhLuan);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = maBv });
    }
    #endregion
}