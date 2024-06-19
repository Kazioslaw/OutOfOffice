using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;

namespace OutOfOfficeHRApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<OutOfOfficeContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("OutOfOfficeContext")
                    ?? throw new InvalidOperationException("Connection string 'OutOfOfficeContext' not found."));
            });

            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "RequestToken";
            });

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(next => context =>
            {
                var tokens = app.Services.GetRequiredService<IAntiforgery>();
                var tokenSet = tokens.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF", tokenSet.RequestToken,
                    new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false });
                return next(context);
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllers();

            app.Run();
        }
    }
}
