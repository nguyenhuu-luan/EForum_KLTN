using AutoMapper;
using EForumKLTN.Models;
using EForumKLTN.Helpers;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EForumKLTN.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly EForumContext db;
        private readonly IMapper _mapper;

        public KhachHangController(EForumContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile? Hinh)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi: {e.ErrorMessage}");
                    }
                }
                return View(model);
            }

            try
            {
                var khachHang = _mapper.Map<KhachHang>(model);

                khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                khachHang.HieuLuc = true;
                khachHang.VaiTro = 0;

                if (Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                Console.WriteLine("Đã lưu DB thành công!");

                return RedirectToAction("Index", "HangHoa");
            }
            catch (Exception ex)
            {
                Console.WriteLine("LỖI DB: " + ex.Message);
            }

            return View(model);
        }
    }
}