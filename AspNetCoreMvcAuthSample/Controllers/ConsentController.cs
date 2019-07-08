using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreMvcAuthSample.Services;
using AspNetCoreMvcAuthSample.ViewModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMvcAuthSample.Controllers
{
	/// <summary>
	/// 授权获取信息页面
	/// Author：Jesen
	/// Date：2019-07-07
	/// </summary>
    public class ConsentController : Controller
    {
		private readonly ConsentService _consentService;
		public ConsentController(ConsentService consentService)
		{
			_consentService = consentService;
		}



		[HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
			var model = await _consentService.BuildConsentViewModelAsync(returnUrl);
			if(model == null)
			{
				return null;
			}

            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> Index(InputConsentViewModel viewModel)
		{
			var result = await _consentService.ProcessConsent(viewModel);

			if (result.IsRedirect)
			{
				return Redirect(result.RedireUrl);
			}
			if (!string.IsNullOrEmpty(result.ValidationError))
			{
				ModelState.AddModelError("", result.ValidationError);
			}
			return View(result.ViewModel);
		}
    }
}