using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Services;
using Geonorge.MinSide.Utils;
using Kartverket.Geonorge.Utilities.LogEntry;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(8);
            });


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                 .AddMvcOptions(x => { x.EnableEndpointRouting = false; })
            //.ConfigureApplicationPartManager(manager =>
            //{
            //    var oldMetadataReferenceFeatureProvider = manager.FeatureProviders.First(f => f is MetadataReferenceFeatureProvider);
            //    manager.FeatureProviders.Remove(oldMetadataReferenceFeatureProvider);
            //    manager.FeatureProviders.Add(new ReferencesMetadataReferenceFeatureProvider());
            //})
            ;

            services.AddHttpContextAccessor();
            services
                .AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.TokenValidationParameters.ValidIssuer = Configuration["auth:oidc:issuer"];
                    options.Authority = Configuration["auth:oidc:authority"];
                    options.ClientId = Configuration["auth:oidc:clientid"];
                    options.ClientSecret = Configuration["auth:oidc:clientsecret"];
                    options.MetadataAddress = Configuration["auth:oidc:metadataaddress"];
                    options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;
                    options.SignedOutRedirectUri = Configuration["PostLogoutRedirectUri"];
                    options.SaveTokens = true;
                    options.EventsType = typeof(GeonorgeOpenIdConnectEvents);
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["auth:oidc:authority"];
                    options.Audience = Configuration["auth:oidc:clientid"];
                    options.MetadataAddress = Configuration["auth:oidc:metadataaddress"];
                });
            
            // authorize both via cookies and jwt bearer tokens
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            
            
            var applicationSettings = new ApplicationSettings();
            Configuration.Bind(applicationSettings);
            services.AddSingleton<ApplicationSettings>(applicationSettings);
            services.AddHttpClient();

            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddDbContext<OrganizationContext>(item => item.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<GeonorgeOpenIdConnectEvents>();
            services.AddTransient<IGeonorgeAuthorizationService, GeonorgeAuthorizationService>();
            services.AddTransient<IBaatAuthzApi, BaatAuthzApi>();

            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IMeetingService, MeetingService>();
            services.AddHostedService<Services.Tasks.TimedHostedService>();
            services.AddSingleton<ILogEntryService>(new LogEntryService(applicationSettings.LogApi, applicationSettings.LogApiKey, new Kartverket.Geonorge.Utilities.Organization.HttpClientFactory()));
            ConfigureProxy(applicationSettings);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var applicationSettings = app.ApplicationServices.GetService<ApplicationSettings>();

            // Add explicit proxy configuration
            // this must be the first "app.Use..."-statement in the Configure-method
            if (!string.IsNullOrEmpty(applicationSettings.ProxyAddress))
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    ForwardLimit = null,
                    KnownProxies = { IPAddress.Parse(applicationSettings.ProxyAddress) }
                });    
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                });    
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                
                // Geonorge proxy does not send correct header - force https scheme
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            /*
                        // Debug Proxy headers
                        app.Use(async (context, next) =>
                        {
                            // Request method, scheme, and path
                            Log.Debug("Request Method: {METHOD}", context.Request.Method);
                            Log.Debug("Request Scheme: {SCHEME}", context.Request.Scheme);
                            Log.Debug("Request Path: {PATH}", context.Request.Path);

                            // Headers
                            foreach (var header in context.Request.Headers)
                            {
                                Log.Debug("Header: {KEY}: {VALUE}", header.Key, header.Value);
                            }

                            // Connection: RemoteIp
                            Log.Debug("Request RemoteIp: {REMOTE_IP_ADDRESS}",
                                context.Connection.RemoteIpAddress);

                            await next();
                        });
            */
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

        private static void ConfigureProxy(ApplicationSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.ProxyAddress))
            {
                WebProxy proxy = new WebProxy(settings.ProxyAddress);

                proxy.Credentials = CredentialCache.DefaultCredentials;

                WebRequest.DefaultWebProxy = proxy;
                HttpClient.DefaultProxy = proxy;
            }
        }

    }
}