using System;
using System.Collections.Generic;

namespace EForumKLTN.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public string MaKh { get; set; } = null!;

    public DateTime? NgayDat { get; set; }

    public string? DiaChi { get; set; }

    public string? CachThanhToan { get; set; }

    public int? MaTrangThai { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
