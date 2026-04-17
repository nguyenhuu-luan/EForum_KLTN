using EForumKLTN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EForumKLTN.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EForumKLTN.Controllers
{
    public class HomeController : Controller
    {
        private readonly EForumContext _db;
        public HomeController(EForumContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ShowIndex()
        {
            var model = new HomeVM
            {
                // Lấy 3 bài viết mới nhất, kèm thông tin người đăng và chủ đề
                TopPosts = await _db.BaiViets
                    .Include(b => b.MaKhNavigation)
                    .Include(b => b.MaCdNavigation)
                    .OrderByDescending(b => b.NgayDang)
                    .Take(3)
                    .ToListAsync(),

                // Lấy 10 sản phẩm bất kỳ hoặc mới nhất
                FeaturedProducts = await _db.HangHoas
                    .Include(s => s.MaLoaiNavigation) // nếu cần TenLoai
                    .OrderByDescending(s => s.MaHh)
                    .Take(10)
                    .Select(s => new HangHoaVM
                    {
                        MaHH = s.MaHh,
                        TenHH = s.TenHh,
                        DonGia = s.DonGia ?? 0,
                        Hinh = s.Hinh ?? "",
                        MoTaNgan = s.MoTa ?? "",
                        TenLoai = s.MaLoaiNavigation.TenLoai,
                    })
                    .ToListAsync(),
            };
            return View(model);
            
        }
    }
}
