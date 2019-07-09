using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreMvcAuthSample.Data;
using AspNetCoreMvcAuthSample.Models;
using AspNetCoreMvcAuthSample.ViewModel;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMvcAuthSample.Controllers
{
    public class AccountController : Controller
    {
		//private readonly TestUserStore _users;

		//public AccountController(TestUserStore users)
		//{
		//	_users = users;
		//}

		private UserManager<ApplicationUser> _userManager;
		private SignInManager<ApplicationUser> _signInManager;
		private IIdentityServerInteractionService _interaction;

		public AccountController(UserManager<ApplicationUser> userManager
			,SignInManager<ApplicationUser> signInManager
			,IIdentityServerInteractionService interaction
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_interaction = interaction;
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
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel,string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				ViewData["ReturnUrl"] = returnUrl;

				var identityUser = new ApplicationUser
                {
					Email = registerViewModel.Email,
					UserName = registerViewModel.Email,
					NormalizedUserName = registerViewModel.Email
				};

				IdentityResult identityResult = await _userManager.CreateAsync(identityUser, registerViewModel.Password);

				if (identityResult.Succeeded)
				{
					await _signInManager.SignInAsync(identityUser, new AuthenticationProperties { IsPersistent = true });
					return Redirect(returnUrl);
				}
				else
				{
					return View(registerViewModel);
					//AddErrors(identityResult);
				}
			}


			return View(registerViewModel);
		}

		[HttpGet]
		public IActionResult Login(string returnUrl="")
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = "")
		{
			if (ModelState.IsValid)
			{
				ViewData["ReturnUrl"] = returnUrl;
				var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

				if (user == null)
				{
					ModelState.AddModelError(nameof(loginViewModel.Email), "Email not exists");
				}
				else
				{
					if (await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
					{
						AuthenticationProperties props = null;
						if (loginViewModel.RememberMe)
						{
							props = new AuthenticationProperties
							{
								IsPersistent = true,
								ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
							};
						}

						await _signInManager.SignInAsync(user, props);

						if (_interaction.IsValidReturnUrl(returnUrl))
						{
							return Redirect(returnUrl);
						}

						return Redirect("/");
					}
					else
					{
						ModelState.AddModelError(nameof(loginViewModel.Password),"Wrong Password");
					}
				}
			}
			//TestUser user = _users.FindByUsername(username);
			//if(user == null)
			//{
			//	ModelState.AddModelError(nameof(username), "username is not exist");
			//}
			//else
			//{
			//	if(_users.ValidateCredentials(username, password))
			//	{
			//		var props = new AuthenticationProperties
			//		{
			//			IsPersistent = true,
			//			ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
			//		};

			//		//HttpContext.SignInAsync(user.SubjectId, user.Username, props);
			//		Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(base.HttpContext,user.SubjectId, user.Username, props);
			//		return Redirect(ReturnUrl);
			//	}

			//	ModelState.AddModelError(nameof(password), "wrong password");
			//}

			return View(loginViewModel);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");

			//base.HttpContext.SignOutAsync().Wait(); // SignOutAsync 注销

			//return this.Redirect("~/Account/Login");
		}

	}
}