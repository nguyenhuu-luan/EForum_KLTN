using EForumKLTN.Models;

namespace EForumKLTN.ViewModels
{
    public class HomeVM
    {
        public List<BaiViet> TopPosts { get; set; }
        public List<HangHoaVM> FeaturedProducts { get; set; }
    }
}
