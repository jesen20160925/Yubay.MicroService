using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridModeAuthSample
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

			services.AddIdentityServer()
				.AddDeveloperSigningCredential()
				.AddInMemoryClients(Config.GetClients())
				.AddInMemoryApiResources(Config.GetResource())
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddTestUsers(Config.GetTestUsers());

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
	}
}
