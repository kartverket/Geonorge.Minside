using System.Linq;
using Geonorge.MinSide.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Geonorge.MinSide
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .ConfigureApplicationPartManager(manager =>
                {
                    var oldMetadataReferenceFeatureProvider = manager.FeatureProviders.First(f => f is MetadataReferenceFeatureProvider);
                    manager.FeatureProviders.Remove(oldMetadataReferenceFeatureProvider);
                    manager.FeatureProviders.Add(new ReferencesMetadataReferenceFeatureProvider());
                });;

            services.AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options => 
                { 
                    options.LoginPath = new PathString("/login"); 
                    options.LogoutPath = new PathString("/logout"); 
                })
                .AddOpenIdConnect(options =>
                    {
                        options.Authority = Configuration["auth:oidc:authority"];
                        options.ClientId = Configuration["auth:oidc:clientid"];
                        options.ClientSecret = Configuration["auth:oidc:clientsecret"];
                        options.MetadataAddress = Configuration["auth:oidc:metadataaddress"];
                        options.SaveTokens = true;
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.GetClaimsFromUserInfoEndpoint = true;
                    }
                );
            var applicationSettings = new ApplicationSettings();
            Configuration.Bind(applicationSettings);
            services.AddSingleton<ApplicationSettings>(applicationSettings);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();

            //app.UseStatusCodePages();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}