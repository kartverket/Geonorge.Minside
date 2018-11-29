using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Utils;
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
using Serilog;

namespace Geonorge.MinSide
{
    public class Startup
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

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
                }); ;

            services
                .AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.Authority = Configuration["auth:oidc:authority"];
                    options.ClientId = Configuration["auth:oidc:clientid"];
                    options.ClientSecret = Configuration["auth:oidc:clientsecret"];
                    options.MetadataAddress = Configuration["auth:oidc:metadataaddress"];
                    options.SaveTokens = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var authorizationService =
                                ctx.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
                            ctx.Principal.AddIdentity(await authorizationService.GetClaims((ClaimsIdentity)ctx.Principal.Identity));
                        }
                    };
                });
            
            var applicationSettings = new ApplicationSettings();
            Configuration.Bind(applicationSettings);
            services.AddSingleton<ApplicationSettings>(applicationSettings);
            services.AddHttpClient();
            services.AddTransient<IAuthorizationService, GeonorgeAuthorizationService>();
            services.AddTransient<IBaatAuthzApi, BaatAuthzApi>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            /*
            // proxy does not send correct header - force https scheme
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            
            */
            
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseForwardedHeaders();

            //// Debug Proxy headers
            //app.Use(async (context, next) =>
            //{
            //    // Request method, scheme, and path
            //    Log.Debug("Request Method: {METHOD}", context.Request.Method);
            //    Log.Debug("Request Scheme: {SCHEME}", context.Request.Scheme);
            //    Log.Debug("Request Path: {PATH}", context.Request.Path);

            //    // Headers
            //    foreach (var header in context.Request.Headers)
            //    {
            //        Log.Debug("Header: {KEY}: {VALUE}", header.Key, header.Value);
            //    }

            //    // Connection: RemoteIp
            //    Log.Debug("Request RemoteIp: {REMOTE_IP_ADDRESS}",
            //        context.Connection.RemoteIpAddress);

            //    await next();
            //});

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