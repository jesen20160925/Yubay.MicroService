using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreMvcAuthSample.Data;
using AspNetCoreMvcAuthSample.Models;
using AspNetCoreMvcAuthSample.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using System.Reflection;
using IdentityServer4.EntityFramework.Mappers;

namespace AspNetCoreMvcAuthSample
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
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddIdentity<ApplicationUser, ApplicationUserRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();


			services.AddIdentityServer()
				.AddDeveloperSigningCredential() //会创建一个用于对token签名的临时密钥材料(但是在生产环境中应该使用可持久的密钥材料)
                //.AddDeveloperSigningCredential("tempkey.rsa") //AddDeveloperSigningCredential中代码执行，会先判断tempkey.rsa证书文件是否存在，如果不存在的话，就创建一个新的tempkey.rsa证书文件，如果存在的话，就使用此证书文件。
				//.AddInMemoryClients(Config.GetClients())
				//.AddInMemoryApiResources(Config.GetResource())
				//.AddInMemoryIdentityResources(Config.GetIdentityResources())

                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(Configuration.GetConnectionString("IdentityServerConnection"), sql => sql.MigrationsAssembly(migrationAssembly));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(Configuration.GetConnectionString("IdentityServerConnection"), sql => sql.MigrationsAssembly(migrationAssembly));
                    };
                })

				//.AddTestUsers(Config.GetTestUsers());
				.AddAspNetIdentity<ApplicationUser>()
                .Services.AddScoped<IProfileService,ProfileService>();

           // IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext

            //配置密码等一些规则
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            });

			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
			//	options.CheckConsentNeeded = context => true;
			//	options.MinimumSameSitePolicy = SameSiteMode.None;
			//});

			//#region 认证注入
			////AuthenticationHttpContextExtensions
			//services.AddAuthentication(options =>
			//{
			//	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//})
			//	.AddCookie(options =>
			//	{
			//		options.ClaimsIssuer = "Cookie";
			//		options.LoginPath = "/Account/Login";//修改默认跳转登录页 默认是/Account/Login ,详情可参考 https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2
			//	});

			//#endregion

			services.AddScoped<ConsentService>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            InitIdentityServerDb(app);

			app.UseStaticFiles();
			app.UseCookiePolicy();

			//app.UseAuthentication();

			app.UseIdentityServer();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}


        public void InitIdentityServerDb(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                    .Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!configurationDbContext.Clients.Any())
                {
                    foreach(var client in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                }
                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetResource())
                    {
                        configurationDbContext.ApiResources.Add(api.ToEntity());
                    }
                }
                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                }

                configurationDbContext.SaveChanges();
            }
        }

	}
}
