using System;
using System.Collections.Generic;

namespace EForumKLTN.Models;

public partial class LichSuChatbot
{
    public int MaChat { get; set; }

    public string? MaKh { get; set; }

    public string? CauHoi { get; set; }

    public string? TraLoiAi { get; set; }

    public DateTime? NgayChat { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }
}
