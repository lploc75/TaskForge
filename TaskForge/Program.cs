using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TaskForge.DBContext;
using TaskForge.Repository;
using TaskForge.Service;
using static Dropbox.Api.TeamLog.EventCategory;

namespace TaskForge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext với SQL Server
            builder.Services.AddDbContext<TaskForgeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocConnection")));

            // Đăng ký các dịch vụ với Dependency Injection
            builder.Services.AddMemoryCache();                       // Thêm In-Memory Cache
            builder.Services.AddScoped<EmailService>();              // Đăng ký EmailService
            builder.Services.AddScoped<AccountService>();            // Đăng ký AccountService
            builder.Services.AddScoped<AccountRepository>();         // Đăng ký AccountRepository nếu cần cho AccountService
            builder.Services.AddScoped<EmployeeService>();           // Đăng ký EmployeeService
            builder.Services.AddScoped<EmployeeRepository>();        // Đăng ký EmployeeRepository
            builder.Services.AddScoped<DropboxService>();            // Đăng ký DropboxService với DI container
            builder.Services.AddScoped<FileRepository>();        // Đăng ký FileRepository

            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();

            // Cấu hình Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";             // Trang đăng nhập
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn của Cookie
                });

            // Thêm dịch vụ Authorization
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();                                       // HSTS cho sản xuất
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Thêm UseAuthentication trước UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Thiết lập route mặc định cho ứng dụng
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Staff}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
