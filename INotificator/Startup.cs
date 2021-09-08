using System.IO;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Interfaces.Services;
using INotificator.Common.Logger;
using INotificator.Common.Models;
using INotificator.Context;
using INotificator.Services;
using INotificator.Services.Parser;
using INotificator.Services.Receivers;
using INotificator.Services.Senders;
using INotificator.Services.Storages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace INotificator
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("AppDbContext")));
            
            services
                .AddScoped<IBackgroundService, NotificationService>()
                .AddScoped<IStorage<Product>, DataBaseStorage>()
                .AddScoped<ISender, TelegramBotService>()
                .AddScoped<IDnsReceiver, DnsReceiver>()
                .AddScoped<IDnsParser, DnsParser>()
                .AddScoped<IDnsService, DnsService>()
                .AddScoped<IHpoolService, HpoolService>()
                .AddScoped<IAvitoReceiver, AvitoReceiver>()
                .AddScoped<IAvitoParser, AvitoParser>()
                .AddScoped<IAvitoService, AvitoService>()
                .AddScoped<IBasicApiParser, BasicApiParser>()
                .AddScoped<IOnlinetradeReceiver, OnlinetradeReceiver>()
                .AddScoped<IOnlinetradeParser, OnlinetradeParser>()
                .AddScoped<IOnlinetradeService, OnlinetradeService>()
                .AddScoped<IBasicApiReceiver, BasicApiReceiver>()
                .AddHttpClient();
            
            services.AddHostedService<AppBackgroundService>();

            services.Configure<Options>(configuration.GetSection("App"));

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();

                var config = new ColorConsoleLoggerConfiguration();
                builder.ClearProviders();
                builder.AddProvider(new ColorConsoleLoggerProvider(config));

            });

            services.AddOptions();

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}