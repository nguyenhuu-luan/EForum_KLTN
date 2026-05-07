using EForumKLTN.Helpers;
using EForumKLTN.Models;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.Controllers
{
    public class CartController : Controller
    {
        private readonly EForumContext db;
        public CartController(EForumContext context) 
        {
            db = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHH == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy sản phẩm mã số {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHH = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHH == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        #region ThanhToan
        [Authorize]
        [HttpPost]
        public IActionResult ThanhToan(string? couponCode)
        {
            var gioHang = Cart;

            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var userId = User.FindFirst("CustomerID")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            double tongTien = gioHang.Sum(p => p.ThanhTien);
            string? maNhanVien = null;

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = db.Coupons
                    .FirstOrDefault(c => c.MaCoupon == couponCode);

                if (coupon != null)
                {
                    maNhanVien = coupon.MaKH_NV;
                }
            }
            var hoaDon = new HoaDon
            {
                MaKh = userId,
                NgayDat = DateTime.Now,
                MaTrangThai = 0,
                MaKH_NV = maNhanVien,
                TongTien = (float)tongTien
            };

            db.HoaDons.Add(hoaDon);
            db.SaveChanges();  

            foreach (var item in gioHang)
            {
                var ct = new ChiTietHd
                {
                    MaHd = hoaDon.MaHd,
                    MaHh = item.MaHH,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };

                db.ChiTietHds.Add(ct);
            }

            db.SaveChanges();
            HttpContext.Session.Remove(MySetting.CART_KEY); //nay` qtrong nha, an thanh toan -> remove cart
            string noiDung = $"HD{hoaDon.MaHd}";

            string bankCode = "970422";  
            string soTaiKhoan = "01904006211923";
            string tenTaiKhoan = "NGUYEN%20HUU%20LUAN";

            string qrUrl = $"https://img.vietqr.io/image/{bankCode}-{soTaiKhoan}-compact2.png?amount={(int)tongTien}&addInfo={Uri.EscapeDataString(noiDung)}&accountName={Uri.EscapeDataString("NGUYEN HUU LUAN")}";
            ViewBag.QR = qrUrl;
            ViewBag.TongTien = tongTien;
            ViewBag.NoiDung = noiDung;

            return View();
        }
        #endregion
    }
}
