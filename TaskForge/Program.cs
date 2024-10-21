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
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<EmployeeRepository>();
            builder.Services.AddScoped<DropboxService>();
            builder.Services.AddScoped<FileRepository>();
            builder.Services.AddScoped<ProjectRepository>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<TaskRepository>();
            builder.Services.AddScoped<TaskService>();

            builder.Services.AddHttpContextAccessor();

            // Thêm dịch vụ Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();

            // Cấu hình Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                });

            // Thêm dịch vụ Authorization
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Kích hoạt session
            app.UseSession();
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