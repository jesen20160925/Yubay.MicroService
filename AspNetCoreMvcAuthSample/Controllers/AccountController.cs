using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreMvcAuthSample.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMvcAuthSample.Controllers
{
    public class AccountController : Controller
    {
		private readonly TestUserStore _users;

		public AccountController(TestUserStore users)
		{
			_users = users;
		}

		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(string email,string password)
		{
			return View();
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(string username, string password, string ReturnUrl = "")
		{
			//User user = UserMock.FindUser(username, password);//这里写自己的认证逻辑

			//var claimIdentity = new ClaimsIdentity("Cookie");
			//claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			//claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
			//claimIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
			//claimIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
			//var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
			//// 在Startup注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。
			//base.HttpContext.SignInAsync(claimsPrincipal).Wait(); //SignInAsync 登入

			//if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);

			TestUser user = _users.FindByUsername(username);
			if(user == null)
			{
				ModelState.AddModelError(nameof(username), "username is not exist");
			}
			else
			{
				if(_users.ValidateCredentials(username, password))
				{
					var props = new AuthenticationProperties
					{
						IsPersistent = true,
						ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
					};

					//HttpContext.SignInAsync(user.SubjectId, user.Username, props);
					Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(base.HttpContext,user.SubjectId, user.Username, props);
					return Redirect(ReturnUrl);
				}

				ModelState.AddModelError(nameof(password), "wrong password");
			}

			return View();
			
		}

		public ActionResult Logout()
		{
			base.HttpContext.SignOutAsync().Wait(); // SignOutAsync 注销

			return this.Redirect("~/Account/Login");
		}

	}
}