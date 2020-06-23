using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using stellar_dotnet_sdk;
using System.Linq;
using WageringGG.Server.Data;
using WageringGG.Server.Models;
using WageringGG.Server.Services;

namespace WageringGG.Server
{
    public class Startup
    {
        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection connections = _config.GetSection("ConnectionStrings");
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connections["Application"]);
            });
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(connections["Identity"]);
            });

            services.AddDefaultIdentity<ApplicationUser>(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.SignIn.RequireConfirmedAccount = !_env.IsDevelopment();
            }).AddEntityFrameworkStores<IdentityDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, IdentityDbContext>()
                .AddProfileService<ProfileService>();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection section =
                        _config.GetSection("Authentication:Google");

                    options.ClientId = section["ClientId"];
                    options.ClientSecret = section["ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    IConfigurationSection section =
                        _config.GetSection("Authentication:Facebook");

                    options.AppId = section["ClientId"];
                    options.AppSecret = section["ClientSecret"];
                    options.AccessDeniedPath = "/Error";
                })
                .AddTwitter(options =>
                {
                    IConfigurationSection section =
                        _config.GetSection("Authentication:Twitter");

                    options.ConsumerKey = section["ClientId"];
                    options.ConsumerSecret = section["ClientSecret"];
                })
                .AddIdentityServerJwt();

            services.Configure<JwtBearerOptions>(
            IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
            options =>
            {
                var received = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    await received(context);
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/group-hub"))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                };
            });

            services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddRazorPages();
            services.AddSignalR().AddAzureSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            if (_env.IsDevelopment())
                Network.UseTestNetwork();
            else
                Network.UsePublicNetwork();
            services.AddSingleton(new stellar_dotnet_sdk.Server(_config["Stellar:URI"]));
            if (!_env.IsDevelopment())
                services.AddScoped<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseFileServer();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Hubs.GroupHub>("/group-hub");
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
