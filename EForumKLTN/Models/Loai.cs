using System;
using System.Collections.Generic;

namespace EForumKLTN.Models;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<BaiViet> BaiViets { get; set; } = new List<BaiViet>();

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
