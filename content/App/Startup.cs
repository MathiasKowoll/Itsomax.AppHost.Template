using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Web;
using Itsomax.Module.Core.Extensions;
using Itsomax.Module.Core.Models;
//using Itsomax.Module.Localization;
//using Itsomax.WebHost.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Itsomax.Module.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;
using Microsoft.ApplicationInsights.Extensibility;


namespace Itsomax.AppHost
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private static readonly IList<ModuleInfo> Modules = new List<ModuleInfo>();

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            var connectionStringConfig = builder.Build();


            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.WebRootPath = _hostingEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _hostingEnvironment.ContentRootPath;
            services.LoadInstalledModules(_hostingEnvironment.ContentRootPath);
            services.LoadHangfire(Configuration);
            //services.LoadToastNotification();

            services.AddCustomizedDataStore(Configuration);
            services.AddCustomizedIdentity(Configuration);
            services.AddCustomizedAuthorization();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddScoped<SignInManager<User>, ItsomaxSignInManager<User>>();
            services.AddCloudscribePagination();

            services.Configure<RazorViewEngineOptions>(
                options => { options.ViewLocationExpanders.Add(new ModuleViewLocationExpander()); });

            services.AddNToastNotify();
            services.AddCustomizedMvc(GlobalConfiguration.Modules);

            return services.Build(Configuration, _hostingEnvironment);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IConfiguration configuration)
        {
            //var config = app.ApplicationServices.GetService<TelemetryConfiguration>();
            //config.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {

                app.UseExceptionHandler("/Web/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Web/ErrorWithCode/{0}");
            app.UseCustomizedRequestLocalization();
            app.UseCustomizedStaticFiles(env);
            app.UseCustomizedIdentity();
            app.UseCustomizedMvc(Configuration);
            SeedInitialdata.CreateDB(app.ApplicationServices).Wait();

            var configDb = configuration.GetSection("UseConnection:DefaultConnection").Value;

            if (configDb == "Postgres")
            {
                SeedInitialdata.CreateExtention(app.ApplicationServices).Wait();
            }
            SeedInitialdata.InitialAppSettings(app.ApplicationServices);
            SeedInitialdata.CreateAdmin(app.ApplicationServices).Wait();
            SeedInitialdata.InitializeModules(app.ApplicationServices);
            app.UseCustomizedHangfire();
            //app.SeedData(Configuration);
        }
    }
}
