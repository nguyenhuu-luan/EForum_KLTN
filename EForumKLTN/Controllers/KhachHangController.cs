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
        private readonly IWebHostEnvironment _env;


        public KhachHangController(EForumContext context, IMapper mapper, IWebHostEnvironment env)
        {
            db = context;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;//sẽ xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                       khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang", _env);
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.ToString()); // 🔥 thêm ở đây
                    return View(model); // 🔥 thêm luôn dòng này
                }
            }
            return View();
        }
    }
} 
