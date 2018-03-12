using CW03.Data;
using CW03.IRepositories;
using CW03.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc();

            services.AddDbContext<CW03Context>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CW03Context")));

            //Adding application services
            services.AddTransient<IBookmarkEntityRepo, BookmarkEntityRepo>();
            services.AddTransient<IFolderRepo, FolderRepo>();            
            services.AddTransient<IItemLinkRepo, ItemLinkRepo>();
            services.AddTransient<IItemLocationRepo, ItemLocationRepo>();
            services.AddTransient<IItemTextfileRepo, ItemTextfileRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
