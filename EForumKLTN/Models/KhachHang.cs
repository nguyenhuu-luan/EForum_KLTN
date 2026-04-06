using System;
using System.Collections.Generic;

namespace EForumKLTN.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string? MatKhau { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DienThoai { get; set; }

    public bool? HieuLuc { get; set; }

    public virtual ICollection<BaiViet> BaiViets { get; set; } = new List<BaiViet>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<LichSuChatbot> LichSuChatbots { get; set; } = new List<LichSuChatbot>();
}
