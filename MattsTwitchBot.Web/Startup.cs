using System.Reflection;
using Couchbase.Extensions.DependencyInjection;
using MattsTwitchBot.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                })
                .AddTwitch(options =>
                {
                    options.ClientId = Configuration.GetValue<string>("Twitch:ApiClientId");
                    options.ClientSecret = Configuration.GetValue<string>("Twitch:ApiClientSecret");
                });

            services.AddHttpContextAccessor();

            services.Configure<TwitchOptions>(Configuration.GetSection("Twitch"));

            services
                .AddCouchbase(Configuration.GetSection("Couchbase"))
                .AddCouchbaseBucket<ITwitchBucketProvider>(Configuration.GetValue<string>("Couchbase:BucketName"));

            services.AddMediatR(Assembly.GetAssembly(typeof(MattsChatBotHostedService)));

            services.AddTransient<TwitchCommandRequestFactory>();
            services.AddSingleton<IHostedService, MattsChatBotHostedService>();
            services.AddSingleton<ITwitchClient>(x =>
            {
                var userName = Configuration.GetValue<string>("Twitch:Username");
                var oauthKey = Configuration.GetValue<string>("Twitch:OauthKey");
                var credentials = new ConnectionCredentials(userName, oauthKey);
                var twitchClient = new TwitchClient();
                twitchClient.Initialize(credentials, userName);
                return twitchClient;
            });
            services.AddSingleton<ITwitchApiWrapper>(x =>
            {
                var apiClientId = Configuration.GetValue<string>("Twitch:ApiClientId");
                var apiClientSecret = Configuration.GetValue<string>("Twitch:ApiClientSecret");
                var api = new TwitchAPI();
                api.Settings.ClientId = apiClientId;
                api.Settings.AccessToken = apiClientSecret;
                return new TwitchApiWrapper(api);
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR();
            services.AddApplicationInsightsTelemetry();

            services.AddTransient<IKeyGenerator, KeyGenerator>();
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

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatWebPageHub>("/twitchHub");
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
