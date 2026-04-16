namespace EForumKLTN.Models
{
    public class ChuDe
    {
        public int MaCd { get; set; }
        public string TenChuDe { get; set; }
        public string MoTa { get; set; }
        public virtual ICollection<BaiViet> BaiViets { get; set; }
    }
}
