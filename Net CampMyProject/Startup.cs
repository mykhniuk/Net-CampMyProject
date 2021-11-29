using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net_CampMyProject.Data;
using System;
using System.Threading.Tasks;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            InitializeAppServices(services);
        }

        private void InitializeAppServices(IServiceCollection services)
        {
            services.AddScoped<IFilmsRepository, FilmsRepository>();
            services.AddScoped<IGenresRepository, GenresRepository>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<IFilmRatingSourcesRepository, FilmRatingSourcesRepository>();
            services.AddScoped<IFilmRatingsRepository, FilmRatingsRepository>();
            services.AddScoped<IFilmGenresRepository, FilmGenresRepository>();
            services.AddScoped<IMyRatingsRepository, MyRatingsRepository>();
            services.AddScoped<IFilmPersonsRepository, FilmPersonsRepository>();
            services.AddScoped<IGenresRepository, GenresRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            UpdateDatabase(app);

            CreateRolesAndUsersAsync(serviceProvider).Wait();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Films}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context?.Database.Migrate();
        }

        private async Task CreateRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            using var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var adminUserName = "admin@admin.com";
            var adminPwd = "kX6TSaW2LQA1!gmY5cqtsQye";

            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                var adminRole = new IdentityRole { Name = Roles.Admin };
                await roleManager.CreateAsync(adminRole);
            }

            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var adminUser = new IdentityUser { UserName = adminUserName };

                var result = await userManager.CreateAsync(adminUser, adminPwd);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }

            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                var role = new IdentityRole { Name = Roles.User };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
