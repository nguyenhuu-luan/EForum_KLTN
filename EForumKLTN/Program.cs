using EForumKLTN.Helpers;
using EForumKLTN.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EForumContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EForumDB"));
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("UNHANDLED: " + e.ExceptionObject.ToString());
};

TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.WriteLine("TASK ERROR: " + e.Exception.ToString());
};


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/KhachHang/DangNhap";
    options.AccessDeniedPath = "/AccessDenied";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=ShowIndex}/{id?}"); //này nha ví dụ để logic xử lý trong hàm index thì chuyển nó về action index cũng đc nha :)) mà lười quá nên để sau đi

app.Run();