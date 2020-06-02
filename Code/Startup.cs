using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCityCakeCo.Data;
using CloudCityCakeCo.Data.Repositories;
using CloudCityCakeCo.Models.DTO;
using CloudCityCakeCo.Services.Implementations;
using CloudCityCakeCo.Services.Interfaces;
using CloudCityCakeCo.Services.NotificationRules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twilio.Rest.Api.V2010.Account;

namespace CloudCityCakeCo
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultSql")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICakeOrderRepository, CakeOrderRepository>();

            services.AddScoped<ICakeOrderService, CakeOrderService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMessagingService, MessagingService>();

            services.AddScoped<IStatusNotificationRule, CompletedNotificationRule>();
            services.AddScoped<IStatusNotificationRule, AcceptedNotificationRule>();
            services.AddScoped<INotificationHandler, NotificationHandler>();

            services.Configure<SendGridAccount>(Configuration.GetSection("SendGridAccount"));
            services.Configure<TwilioAccount>(Configuration.GetSection("TwilioAccount"));
            
            services.AddControllersWithViews();
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
