namespace EForumKLTN.Models
{
    public class Coupon
    {
        public string MaCoupon { get; set; }
        public string MaKH_NV { get; set; }
        public DateTime NgayTao { get; set; }

        public virtual KhachHang KhachHang { get; set; } = null!;
    }
}
