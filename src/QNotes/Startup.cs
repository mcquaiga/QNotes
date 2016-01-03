using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QNotes.API.Settings;
using QNotes.API.Models;
using QNotes.API.Data.Services;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.Extensions.WebEncoders;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Owin.Security.Jwt;

namespace QNotes
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json") 
                .AddUserSecrets()           
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthentication(options => 
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            // Add framework services.
            services.AddMvc();
            services.AddOptions();

            services.Configure<StorageConfig>(Configuration.GetSection("StorageConfig"));
            services.AddSingleton<IConnectionHandler<Note>, MongoConnectionHandler<Note>>();
            services.AddSingleton<IConnectionHandler<IdentityUser>, MongoConnectionHandler<IdentityUser>>();
            services.AddScoped<IEntityService<Note>, NoteService>();
            services.AddScoped<IEntityService<IdentityUser>, IdentityUserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();
            app.UseStaticFiles();   
            app.UseMvc();

            // Simple error page to avoid a repo dependency.
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    if (context.Response.HasStarted)
                    {
                        throw;
                    }
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(ex.ToString());
                }
            });

            app.UseCookieAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
                options.LoginPath = new PathString("/login");
            });

            // See config.json
            // https://console.developers.google.com/project
            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = Configuration["google:clientid"];
                options.ClientSecret = Configuration["google:clientsecret"];
                options.Events = new OAuthEvents()
                {
                    OnRemoteError = ctx =>
                    {
                        ctx.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.UrlEncode(ctx.Error.Message));
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    }
                };

            });

            app.UseJwtBearerAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
                // You also need to update /wwwroot/app/scripts/app.js
            });

            //// See config.json
            //// https://github.com/settings/applications/
            //app.UseOAuthAuthentication(options =>
            //{
            //    options.AuthenticationScheme = "GitHub-AccessToken";
            //    options.DisplayName = "Github-AccessToken";
            //    options.ClientId = Configuration["github-token:clientid"];
            //    options.ClientSecret = Configuration["github-token:clientsecret"];
            //    options.CallbackPath = new PathString("/signin-github-token");
            //    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            //    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            //    options.SaveTokensAsClaims = true;
            //});

            // Choose an authentication type
            

            // Sign-out to remove the user cookie.
            app.Map("/logout", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });

            // Display the remote error
            app.Map("/error", errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });

            // Deny anonymous request beyond this point.
            app.Use(async (context, next) =>
            {
                if (!context.User.Identities.Any(identity => identity.IsAuthenticated))
                {
                    // The cookie middleware will intercept this 401 and redirect to /login
                    await context.Authentication.ChallengeAsync();
                    return;
                }
                await next();
            });

            // Display user information
            app.Run(async context =>
            {
                IdentityUser existingUser = null;
                var userService = context.RequestServices.GetService<IEntityService<IdentityUser>>() as IdentityUserService;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>");
                await context.Response.WriteAsync("Hello " + (context.User.Identity.Name ?? "anonymous") + "<br>");
                foreach (var claim in context.User.Claims)
                {
                    await context.Response.WriteAsync(claim.Type + ": " + claim.Value + "<br>");
                    if (claim.Type.ToLower().Contains("emailaddress"))
                    {
                        existingUser = await userService.FindUserByEmail(claim.Value);
                    }
                }

                if (existingUser == null)
                {
                    var user = IdentityUser.CreateUserFromClaim(context.User.Claims);
                    await userService.CreateAsync(user);
                    await context.Response.WriteAsync("A new account has been created for you. Enjoy!<br>");
                    await context.Response.WriteAsync(user.ToString() + "<br>");
                }
                await context.Response.WriteAsync("<a href=\"/logout\">Logout</a>");
                await context.Response.WriteAsync("</body></html>");  
            });
    }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
