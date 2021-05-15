using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using web.professionaltranslator.net.Areas.Blog.Services;
using web.professionaltranslator.net.Areas.Identity.Data;
using WebEssentials.AspNetCore.OutputCaching;
using WilderMinds.MetaWeblog;
using MetaWeblogService = web.professionaltranslator.net.Areas.Blog.Services.MetaWeblogService;

namespace web.professionaltranslator.net
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
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddDbContext<EfContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlServerConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<EfContext>();

            services.AddRazorPages();

            //.AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddAreaPageRoute("Admin", "/Testimonial", "Testimonial/{currentPage?}/{withTestimonials?}");
            //    options.Conventions.AddAreaPageRoute("Admin", "/Portfolio", "Portfolio/{currentPage?}/{showApproved?}");

            //});

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddOptions();
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddConfiguration<SiteSettings>(Configuration, "SiteSettings");
            services.AddConfiguration<AdminPortfolioSettings>(Configuration, "AdminPortfolioSettings");

            services.AddSingleton<IBlogService, FileBlogService>();
            services.Configure<BlogSettings>(Configuration.GetSection("Blog"));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMetaWeblog<MetaWeblogService>();

            // Progressive Web Apps https://github.com/madskristensen/WebEssentials.AspNetCore.ServiceWorker
            services.AddProgressiveWebApp(
                new WebEssentials.AspNetCore.Pwa.PwaOptions
                {
                    OfflineRoute = "/shared/offline/"
                });

            // Output caching (https://github.com/madskristensen/WebEssentials.AspNetCore.OutputCaching)
            services.AddOutputCaching(
                options =>
                {
                    options.Profiles["default"] = new OutputCacheProfile
                    {
                        Duration = 3600
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }

    public static class ConfigurationExtension
    {
        public static void AddConfiguration<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationTag = null)
            where T : class
        {
            if (string.IsNullOrEmpty(configurationTag))
            {
                configurationTag = typeof(T).Name;
            }

            var instance = Activator.CreateInstance<T>();
            new ConfigureFromConfigurationOptions<T>(configuration.GetSection(configurationTag)).Configure(instance);
            services.AddSingleton(instance);
        }
    }
}
