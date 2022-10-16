using AzureStorageLibrary;
using AzureStorageLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcWebApplication.Hubs;

namespace MvcWebApplication
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
            ConnectionStrings.AzureStorageConnectionString = Configuration.GetSection("AzureConnectionStrings")["CloudStorageConnectionString"];

            services.AddScoped(typeof(INoSqlStorage<>), typeof(NoSqlStorage<>));
            services.AddSingleton<IBlobStorage, BlobStorage>();

            services.AddControllersWithViews();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/NitificationHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Pictures}/{action=Index}/{id?}");
            });
        }
    }
}