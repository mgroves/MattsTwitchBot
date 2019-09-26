using System;
using System.Collections.Generic;
using System.Reflection;
using Couchbase.Extensions.DependencyInjection;
using MattsTwitchBot.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Web
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

            services
                .AddCouchbase(x =>
                {
                    x.Username = "Administrator";
                    x.Password = "password";
                    x.Servers = new List<Uri> {new Uri("http://localhost:8091")};
                })
                .AddCouchbaseBucket<ITwitchBucketProvider>("twitchchat");

            services.AddMediatR(Assembly.GetAssembly(typeof(MattsChatBotHostedService)));

            services.AddSingleton<IHostedService, MattsChatBotHostedService>();
            services.AddSingleton<ITwitchClient>(x =>
            {
                var credentials = new ConnectionCredentials(Config.Username, Config.OauthKey);
                var twitchClient = new TwitchClient();
                twitchClient.Initialize(credentials, "matthewdgroves");
                return twitchClient;
            });
            services.AddSingleton<ITwitchApiWrapper>(x =>
            {
                var api = new TwitchAPI();
                api.Settings.ClientId = Config.ApiClientId;
                api.Settings.AccessToken = Config.ApiClientSecret;
                return new TwitchApiWrapper(api);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
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
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TwitchHub>("/twitchHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });
        }
    }
}
