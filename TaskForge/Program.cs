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
            builder.Services.AddDbContext<TaskForgeDBContext>(options =>
                options.UseSqlServer("Data Source=LAPTOP-LL;Initial Catalog=TaskForge;User ID=sa;Password=admin@123;TrustServerCertificate=True;"));

            // Register AccountService and any required dependencies
            builder.Services.AddScoped<AccountService>();       // Register AccountService
            builder.Services.AddScoped<AccountRepository>();    // Register AccountRepository if needed by AccountService

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
