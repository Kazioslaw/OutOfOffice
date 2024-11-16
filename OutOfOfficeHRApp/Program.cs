using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);


			builder.Services.AddAuthorization();
			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<OutOfOfficeContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("OutOfOfficeContext")
					?? throw new InvalidOperationException("Connection string 'OutOfOfficeContext' not found."));
			});

			builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<Role>()
				.AddEntityFrameworkStores<OutOfOfficeContext>()
				.AddDefaultTokenProviders();

			//builder.Services.AddAntiforgery();
			builder.Services.AddScoped<Utilities>();

			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapControllers();
			app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>();

				var roles = new[] { "Admin", "Project Manager", "HR Manager", "Employee" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new Role(role));
					}
				}
			}

			using (var scope = app.Services.CreateScope())
			{
				var userManager = scope.ServiceProvider.GetService<UserManager<User>>();

				string adminEmail = "admin@account.com";
				string adminPassword = "Adm!n1";

				if (await userManager.FindByEmailAsync(adminEmail) == null)
				{
					var user = new User();
					user.UserName = "Administrator";
					user.Email = adminEmail;
					user.EmailConfirmed = true;
					await userManager.CreateAsync(user, adminPassword);
					await userManager.AddToRoleAsync(user, "Admin");
				}

				string HRManagerEmail = "HRManager@account.com";
				string HRManagerPassword = "HRM@nager1";

				if (await userManager.FindByEmailAsync(HRManagerEmail) == null)
				{
					var user = new User();
					user.UserName = "HRManager";
					user.Email = HRManagerEmail;
					user.EmailConfirmed = true;
					await userManager.CreateAsync(user, HRManagerPassword);
					await userManager.AddToRoleAsync(user, "HR Manager");
				}

				string employeeEmail = "employee@account.com";
				string employeePassword = "Employ#3";

				if (await userManager.FindByEmailAsync(employeeEmail) == null)
				{
					var user = new User();
					user.UserName = "Employee";
					user.Email = employeeEmail;
					user.EmailConfirmed = true;
					await userManager.CreateAsync(user, employeePassword);
					await userManager.AddToRoleAsync(user, "Employee");
				}

				string projectManagerEmail = "projectManager@account.com";
				string projectManagerPassword = "ProjectM@nager1";

				if (await userManager.FindByEmailAsync(projectManagerEmail) == null)
				{
					var user = new User();
					user.UserName = "ProjectManager";
					user.Email = projectManagerEmail;
					user.EmailConfirmed = true;
					await userManager.CreateAsync(user, projectManagerPassword);
					await userManager.AddToRoleAsync(user, "Project Manager");
				}

				var userTemp = new User();
				userTemp.UserName = "Bogumil_Nowak";
				userTemp.Email = "bogumilnowak@site.com";
				userTemp.EmailConfirmed = true;
				await userManager.CreateAsync(userTemp, "P@ssword1");
				await userManager.AddToRoleAsync(userTemp, "HR Manager");
			}

			app.Run();
		}
	}
}
