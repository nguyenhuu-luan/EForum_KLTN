using EForumKLTN.Helpers;
using EForumKLTN.Models;
using EForumKLTN.Services;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.Controllers
{
    public class CartController : Controller
    {
        private readonly CheckDiscount _discountService;

        private readonly EForumContext db;
        public CartController(EForumContext context, CheckDiscount discountService) 
        {
            db = context;
            _discountService = discountService;
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
                return RedirectToAction("Index");

            var userId = User.FindFirst("CustomerID")?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("DangNhap", "KhachHang");

            var result = _discountService.Calculate(gioHang, couponCode);

            var hoaDon = new HoaDon
            {
                MaKh = userId,
                NgayDat = DateTime.Now,
                MaTrangThai = 0,
                MaKH_NV = result.maNhanVien,
                TongTien = (float)result.finalTotal  
            };

            db.HoaDons.Add(hoaDon);
            db.SaveChanges();

            foreach (var item in gioHang)
            {
                db.ChiTietHds.Add(new ChiTietHd
                {
                    MaHd = hoaDon.MaHd,
                    MaHh = item.MaHH,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                });
            }

            db.SaveChanges();

            HttpContext.Session.Remove(MySetting.CART_KEY);

            string noiDung = $"HD{hoaDon.MaHd}";

            string qrUrl =
                $"https://img.vietqr.io/image/970422-01904006211923-compact2.png" +
                $"?amount={(int)result.finalTotal}" +
                $"&addInfo={Uri.EscapeDataString(noiDung)}";

            ViewBag.QR = qrUrl;
            ViewBag.Total = result.total;
            ViewBag.Discount = result.discount;
            ViewBag.FinalTotal = result.finalTotal;
            ViewBag.TongTien = result.finalTotal;
            ViewBag.NoiDung = noiDung;

            return View();
        }        
        #endregion

        #region Checkdiscount
        [HttpGet]
        public IActionResult CheckCoupon(string code)
        {
            var result = _discountService.Calculate(Cart, code);

            if (result.maNhanVien == null && !string.IsNullOrEmpty(code))
            {
                return Json(new
                {
                    valid = false,
                    discount = 0,
                    total = result.total,
                    finalTotal = result.finalTotal,
                    message = "Mã không hợp lệ"
                });
            }

            return Json(new
            {
                valid = true,
                discount = result.discount,
                total = result.total,
                finalTotal = result.finalTotal
            });
        }
        #endregion
    }
}
