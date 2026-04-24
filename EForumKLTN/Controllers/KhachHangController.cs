using AutoMapper;
using EForumKLTN.Helpers;
using EForumKLTN.Models;
using EForumKLTN.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        #region Register
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
                    khachHang.HieuLuc = true; 
                    khachHang.VaiTro = 0;
                    khachHang.IsAdmin = false;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                       khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang", _env);
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("DangNhap");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.ToString());  
                    return View(model);  
                }
            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if(ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if(khachHang == null)
                {
                    ModelState.AddModelError("Lỗi", "Tài khoản không tồn tại!");
                } 
                else
                {
                    if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                    {
                        ModelState.AddModelError("Lỗi", "Sai thông tin đăng nhập!");
                    } else
                    {
                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Email, khachHang.Email),
                            new Claim(ClaimTypes.Name, khachHang.HoTen),
                            new Claim("CustomerID", khachHang.MaKh),
                            new Claim("Hinh", khachHang.Hinh ?? "default-user.png"),
                            new Claim(ClaimTypes.Role, khachHang.IsAdmin ? "Admin" : "Customer")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            var customerId = User.FindFirst("CustomerID")?.Value;
            var user = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

    }
} 
