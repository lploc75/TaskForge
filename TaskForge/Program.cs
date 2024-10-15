using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TaskForge.DBContext;
using TaskForge.Repository;
using TaskForge.Service;

namespace TaskForge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext với SQL Server
            builder.Services.AddDbContext<TaskForgeContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register AccountService and any required dependencies
            builder.Services.AddScoped<AccountService>();       // Register AccountService
            builder.Services.AddScoped<AccountRepository>();    // Register AccountRepository if needed by AccountService
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<EmployeeRepository>();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            
            // Cấu hình Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            // Thêm dịch vụ Authorization
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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
