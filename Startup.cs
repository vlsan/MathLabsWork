using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMathModelLabs.Data;
using WebMathModelLabs.Data.Repository;
using WebMathModelLabs.Entity;
using WebMathModelLabs.LabCore.BLL;

namespace WebMathModelLabs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            // Set up configuration sources.
            //var builder = new ConfigurationBuilder()
            //.AddJsonFile("appsettings.json")
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);     

            Configuration = configuration;
               // using (var db = new LabDataContext())
           // {
           //          db.Database.EnsureCreated();
          //           db.Database.Migrate();
          //  }
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddEntityFrameworkSqlite().AddDbContext<LabDataContext>();
            services.AddTransient<ILabService, LabDatamanager>();
            services.AddTransient<IRepository<MathLabTable1>, Repository<MathLabTable1>>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
