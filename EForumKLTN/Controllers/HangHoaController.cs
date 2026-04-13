using EForumKLTN.Models;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)             
                .SingleOrDefault(p => p.MaHh == id);
            if(data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm mã số {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? "",
                MoTaNgan = data.MoTa ?? "",
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10, // cái này dữ liệu fake để test mai mốt sẽ làm 1 bảng check số luog sau
                DiemDanhGia = "4.3/5", //cái này cũng fake mai mốt làm chức năng sẽ input = data do user chọn
            };

            return View(result);
        }
    }
}
