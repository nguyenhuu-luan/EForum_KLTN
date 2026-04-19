using EForumKLTN.Models;
using Microsoft.AspNetCore.Authentication.Cookies; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EForumKLTN.ViewModels;


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
   
    #region UserManagement

    public IActionResult ManageUsers()
    {
        var users = _db.KhachHangs.ToList();

        return View(users);
    }


    public IActionResult EditUser(string id)
    {
        var user = _db.KhachHangs.FirstOrDefault(x => x.MaKh == id);

        if (user == null)
            return NotFound();

        var vm = new EditUserVM
        {
            MaKh = user.MaKh,
            HoTen = user.HoTen,
            DienThoai = user.DienThoai,
            IsAdmin = user.IsAdmin
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult EditUser(EditUserVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _db.KhachHangs.FirstOrDefault(x => x.MaKh == model.MaKh);

        if (user == null)
            return NotFound();

        user.HoTen = model.HoTen;
        user.DienThoai = model.DienThoai;
        user.IsAdmin = model.IsAdmin;

        _db.SaveChanges();

        TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
        return RedirectToAction("ManageUsers");
    }


    public IActionResult DeleteUser(string id)
    {
        if (id == null)
            return NotFound();

        // lấy đúng user có MaKh này
        var user = _db.KhachHangs
                           .FirstOrDefault(x => x.MaKh == id);

        if (user == null)
            return NotFound();

        _db.KhachHangs.Remove(user);  
        _db.SaveChanges();

        TempData["SuccessMessage"] = "Xóa người dùng thành công!";
        return RedirectToAction("ManageUsers");
    }
    #endregion

    #region PostManagement

    public IActionResult ManagePosts()
    {
        var posts = _db.BaiViets.ToList();

        return View(posts);
    }

    public IActionResult EditPost(int id)
    {
        var post = _db.BaiViets
                      .FirstOrDefault(x => x.MaBv == id);

        if (post == null)
            return NotFound();

        return View(post);
    }

    [HttpPost]
    public IActionResult EditPost(BaiViet model)
    {
        var post = _db.BaiViets
                      .FirstOrDefault(x => x.MaBv == model.MaBv);

        if (post == null)
            return NotFound();

        post.TieuDe = model.TieuDe;

        _db.SaveChanges();

        TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";

        return RedirectToAction("ManagePosts");
    }

    public IActionResult DeletePost(int id)
    {
        var post = _db.BaiViets
                      .FirstOrDefault(x => x.MaBv == id);

        if (post == null)
            return NotFound();

        _db.BaiViets.Remove(post);

        _db.SaveChanges();

        TempData["SuccessMessage"] = "Xóa bài viết thành công!";

        return RedirectToAction("ManagePosts");
    }

    #endregion

    #region ProductManagement

    public IActionResult ManageProducts()
    {
        // Lấy dữ liệu từ DB và ép kiểu (Map) sang HangHoaVM
        var products = _db.HangHoas
            .Include(p => p.MaLoaiNavigation)
            .Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh ?? "default.png",
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTa ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            })
            .ToList(); // Lúc này kết quả trả về là List<HangHoaVM>

        return View(products);
    }

    public IActionResult CreateProduct()
    {
        // Load danh mục sách cho dropdown
        ViewBag.LoaiList = _db.Loais.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductVM model)
    {
        if (ModelState.IsValid)
        {
            var hinhName = "default.png";
            if (model.ImageFile != null)
            {
                hinhName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "SanPham", hinhName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }

            var hh = new HangHoa
            {
                TenHh = model.TenHh,
                MaLoai = model.MaLoai,
                DonGia = model.DonGia,
                MoTa = model.MoTa,
                Hinh = hinhName
            };

            _db.HangHoas.Add(hh);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm sách mới thành công!";
            return RedirectToAction("ManageProducts");
        }
        ViewBag.LoaiList = _db.Loais.ToList();
        return View(model);
    }

    public IActionResult EditProduct(int id)
    {
        var hh = _db.HangHoas.FirstOrDefault(p => p.MaHh == id);
        if (hh == null) return NotFound();

        var vm = new ProductVM
        {
            MaHh = hh.MaHh,
            TenHh = hh.TenHh,
            MaLoai = hh.MaLoai,
            DonGia = hh.DonGia,
            MoTa = hh.MoTa,
            Hinh = hh.Hinh
        };

        ViewBag.LoaiList = _db.Loais.ToList();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(ProductVM model)
    {
        var hh = _db.HangHoas.FirstOrDefault(p => p.MaHh == model.MaHh);
        if (hh == null) return NotFound();

        if (ModelState.IsValid)
        {
            if (model.ImageFile != null)
            {
                // Upload ảnh mới
                var hinhName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "SanPham", hinhName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                hh.Hinh = hinhName;
            }

            hh.TenHh = model.TenHh;
            hh.MaLoai = model.MaLoai;
            hh.DonGia = model.DonGia;
            hh.MoTa = model.MoTa;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("ManageProducts");
        }
        ViewBag.LoaiList = _db.Loais.ToList();
        return View(model);
    }

    #endregion

}