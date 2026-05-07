using EForumKLTN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EForumKLTN.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class NhanVienController : Controller
    {
        private readonly EForumContext db;

        public NhanVienController(EForumContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var dsHoaDon = db.HoaDons
                .Include(h => h.MaKhNavigation)
                .OrderByDescending(h => h.NgayDat)
                .ToList();

            return View(dsHoaDon);
        }

        public IActionResult XacNhanThanhToan(int id)
        {
            var hoaDon = db.HoaDons.FirstOrDefault(h => h.MaHd == id);

            if (hoaDon == null)
            {
                return NotFound();
            }
            if (hoaDon.MaTrangThai == 1)
            {
                TempData["SuccessMessage"] = "Đơn hàng đã được xác nhận trước đó!";
                return RedirectToAction("Index");
            }

            hoaDon.MaTrangThai = 1;

            if (!string.IsNullOrEmpty(hoaDon.MaKH_NV))
            {
                var nv = db.KhachHangs
                    .FirstOrDefault(x => x.MaKh == hoaDon.MaKH_NV);

                if (nv != null)
                {
                    nv.KPI += 1;
                }
            } // +kpi cho may th nhan vien :))

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã xác nhận thanh toán thành công!";

            return RedirectToAction("Index");
        }
    }
}