namespace EForumKLTN.ViewModels
{
    public class HangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }

    }

    public class ChiTietHangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }

        public string DiemDanhGia { get; set; } 
        public int SoLuongTon { get; set; } //số lượng tồn (số lượng còn lại trong kho)

    }

    public class ProductVM // này xài cho crud của thằng admin nè
    {
        public int MaHh { get; set; }
        public string TenHh { get; set; }
        public int MaLoai { get; set; }
        public double? DonGia { get; set; }
        public string? MoTa { get; set; }
        public string? Hinh { get; set; } // Lưu tên file hiện tại
        public IFormFile? ImageFile { get; set; } // upload hinh 
    }
}
