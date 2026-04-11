using EForumKLTN.Models;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly EForumContext db;
        public MenuLoaiViewComponent(EForumContext context) => db = context;
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.TenLoai); // này có thể mốt sẽ đổi thành mã, để xem mặt hàng nó nên sắp xếp như nào đã 
            return View(data);
        }
    }
}
