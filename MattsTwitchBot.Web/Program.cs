using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;
using Couchbase.Extensions.DependencyInjection;
using MattsTwitchBot.Core;
using MediatR;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddAuthentication(options =>
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
                    options.ClientId = builder.Configuration.GetValue<string>("Twitch:ApiClientId");
                    options.ClientSecret = builder.Configuration.GetValue<string>("Twitch:ApiClientSecret");
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<TwitchOptions>(builder.Configuration.GetSection("Twitch"));

            builder.Services
                .AddCouchbase(builder.Configuration.GetSection("Couchbase"))
                .AddCouchbaseBucket<ITwitchBucketProvider>(builder.Configuration.GetValue<string>("Couchbase:BucketName"));

            builder.Services.AddMediatR(Assembly.GetAssembly(typeof(MattsChatBotHostedService)));

            builder.Services.AddTransient<TwitchCommandRequestFactory>();
            builder.Services.AddHostedService<MattsChatBotHostedService>();
            //services.AddSingleton<IHostedService, MattsChatBotHostedService>();
            builder.Services.AddSingleton<ITwitchClient>(x =>
            {
                var userName = builder.Configuration.GetValue<string>("Twitch:Username");
                var oauthKey = builder.Configuration.GetValue<string>("Twitch:OauthKey");
                var credentials = new ConnectionCredentials(userName, oauthKey);
                var twitchClient = new TwitchClient();
                twitchClient.Initialize(credentials, userName);
                return twitchClient;
            });
            builder.Services.AddSingleton<ITwitchApiWrapper>(x =>
            {
                var apiClientId = builder.Configuration.GetValue<string>("Twitch:ApiClientId");
                var apiClientSecret = builder.Configuration.GetValue<string>("Twitch:ApiClientSecret");
                var api = new TwitchAPI();
                api.Settings.ClientId = apiClientId;
                api.Settings.Secret = apiClientSecret;
                return new TwitchApiWrapper(api);
            });

            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddSignalR();

            builder.Services.AddTransient<IKeyGenerator, KeyGenerator>();







            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatWebPageHub>("/twitchHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Lifetime.ApplicationStopped.Register(() =>
            {
                app.Services.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });

            app.Run();
        }
    }
}