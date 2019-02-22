using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using HybridClient.Auths;
using HybridClient.Requirements;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HybridClient
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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AccessDeniedPath = "/Authorization/AccessDenied";
                    })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "hybrid client";
                    options.ClientSecret = "hybrid secret";
                    options.SaveTokens = true;
                    options.ResponseType = "code id_token";

                    options.Scope.Clear();

                    options.Scope.Add("api1");
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                    options.Scope.Add(OidcConstants.StandardScopes.Email);
                    options.Scope.Add(OidcConstants.StandardScopes.Phone);
                    options.Scope.Add(OidcConstants.StandardScopes.Address);
                    options.Scope.Add("roles");
                    options.Scope.Add("locations");

                    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

                    // 集合里的东西 都是要被过滤掉的属性，nbf amr exp...
                    options.ClaimActions.Remove("nbf");
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.Remove("exp");

                    // 不映射到User Claims里
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("idp");

                    // 让Claim里面的角色成为mvc系统识别的角色
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("SmithInSomewhere", builder =>
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith");
                //    builder.RequireClaim("location", "somewhere");
                //});
                options.AddPolicy("SmithInSomewhere", builder =>
                    {
                        builder.AddRequirements(new SmithInSomewareRequirement());
                    });
            });

            services.AddSingleton<IAuthorizationHandler, SmithInSomewhereHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
