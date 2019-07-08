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
		private UserManager<IdentityUser> _userManger;
		private static ApplicationDbContext context;

		public async Task SeedAsync(IServiceProvider services)
		{
			context = (ApplicationDbContext)services.GetService(typeof(ApplicationDbContext));
			if (!context.Users.Any())
			{
				_userManger = services.GetRequiredService<UserManager<IdentityUser>>();

				var defaultUser = new IdentityUser
				{
					UserName = "Administrator",
					Email = "237745378@qq.com",
					NormalizedUserName = "admin"
				};

				var result = await _userManger.CreateAsync(defaultUser, "Password$123");
				if (!result.Succeeded)
				{
					throw new Exception("Init Default User Failed");
				}
			}
		}
	}
}
