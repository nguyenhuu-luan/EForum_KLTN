using EForumKLTN.Models;
using EForumKLTN.ViewModels;

namespace EForumKLTN.Services
{
    public class CheckDiscount
    {
        private readonly EForumContext db;
        private const double DiscountRate = 0.3;

        public CheckDiscount(EForumContext context)
        {
            db = context;
        }

        public (double total, double discount, double finalTotal, string? maNhanVien, bool validCoupon)
            Calculate(List<CartItem> cart, string? couponCode)
        {
            if (cart == null || cart.Count == 0)
                return (0, 0, 0, null, false);

            double total = cart.Sum(x => x.ThanhTien);

            double discount = 0;
            double finalTotal = total;
            string? maNhanVien = null;
            bool validCoupon = false;

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = db.Coupons.FirstOrDefault(c => c.MaCoupon == couponCode);

                if (coupon != null)
                {
                    validCoupon = true;
                    maNhanVien = coupon.MaKH_NV;

                    discount = total * DiscountRate;
                    finalTotal = total - discount;
                }
            }

            return (total, discount, finalTotal, maNhanVien, validCoupon);
        }
    }
}