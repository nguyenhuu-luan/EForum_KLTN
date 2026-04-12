using EForumKLTN.Models;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly EForumContext db;

        public HangHoaController(EForumContext context)
        {
            db = context;
        }
        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable(); 
            if(loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,  
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTa ?? "", 
                TenLoai = p.MaLoaiNavigation.TenLoai,
            });

            return View(result);
        }

        public IActionResult Search (string? query) {
            var hangHoas = db.HangHoas.AsQueryable();
            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTa ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
            });

            return View(result);
        }
    }
}
