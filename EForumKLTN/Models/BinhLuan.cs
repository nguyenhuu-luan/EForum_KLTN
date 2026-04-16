namespace EForumKLTN.Models
{
    public class BinhLuan
    {
        public int MaBl { get; set; }
        public int MaBv { get; set; }
        public string MaKh { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayBl { get; set; }
        public virtual BaiViet MaBvNavigation { get; set; } = null!;
        public virtual KhachHang MaKhNavigation { get; set; } = null!;
    }
}
