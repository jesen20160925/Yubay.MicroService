using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCClient.Models;

namespace MVCClient.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        /// <summary>
        /// 将清除本地cookie，然后重定向到IdentityServer。IdentityServer将清除其cookie，然后为用户提供返回MVC应用程序的链接。
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
	}
}
