using CW03.Data;
using CW03.IRepositories;
using CW03.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CW03
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CW03Context>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CW03Context")));

            services.AddSingleton(Configuration);
            services.AddControllersWithViews();
            //Adding application services
            services.AddTransient<IBookmarkEntityRepo, BookmarkEntityRepo>();
            services.AddTransient<IFolderRepo, FolderRepo>();
            services.AddTransient<IItemLinkRepo, ItemLinkRepo>();
            services.AddTransient<IItemLocationRepo, ItemLocationRepo>();
            services.AddTransient<IItemTextfileRepo, ItemTextfileRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CW03Context dataContext)
        {
            if (env.IsDevelopment())
            {
                // migrate any database changes on startup (includes initial db creation)
                dataContext.Database.Migrate();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRouting();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
