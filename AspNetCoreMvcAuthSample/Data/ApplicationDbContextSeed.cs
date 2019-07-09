using AspNetCoreMvcAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample.Data
{
	public class ApplicationDbContextSeed
	{
		private UserManager<ApplicationUser> _userManger;
        private RoleManager<ApplicationUserRole> _roleManager;
		private static ApplicationDbContext context;

		public async Task SeedAsync(IServiceProvider services)
		{
			context = (ApplicationDbContext)services.GetService(typeof(ApplicationDbContext));

            if (!context.Roles.Any())
            {
                _roleManager = services.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole() { Name = "Administrators",NormalizedName= "Administrators" };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("Init Default Role Failed");
                }
            }

			if (!context.Users.Any())
			{
				_userManger = services.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
					UserName = "Administrator",
					Email = "237745378@qq.com",
					NormalizedUserName = "admin",
                    Avatar = "http://localhost:5000/img/logo.jpg",
                    SecurityStamp = "admin"
                };

                var result = await _userManger.CreateAsync(defaultUser, "Password$123");
                result = await _userManger.AddToRoleAsync(defaultUser, "Administrators");
                if (!result.Succeeded)
				{
					throw new Exception("Init Default User Failed");
				}
			}
		}
	}
}
