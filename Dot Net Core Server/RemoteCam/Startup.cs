using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RemoteCam.Hubs;
using RemoteCam.Jobs;

namespace RemoteCam
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<CaptureJob>();
            services.AddTransient<WatchInboxJob>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifeTime,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddFile("Logs/RemoteCam-{Date}.log");

            app.UseCors("CorsPolicy");

            var quartzConfig = new QuartzStartup(app);
            appLifeTime.ApplicationStarted.Register(quartzConfig.Start);
            appLifeTime.ApplicationStopping.Register(quartzConfig.Stop);

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<CamHub>("/cam");
            });
        }
    }
}
