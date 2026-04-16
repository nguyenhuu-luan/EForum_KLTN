using System;
using System.Collections.Generic;

namespace EForumKLTN.Models;

public partial class BaiViet
{
    public int MaBv { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? NoiDung { get; set; }

    public DateTime? NgayDang { get; set; }

    public string? MaKh { get; set; }

    public int? MaCd { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual ChuDe? MaCdNavigation { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
}

