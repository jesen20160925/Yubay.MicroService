using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		private readonly IClientStore _clientStore;

		private readonly IResourceStore _resourceStore;

		private readonly IIdentityServerInteractionService _identityServerInteractionService;

		public ConsentController(IClientStore clientStore
			, IResourceStore resourceStore
			, IIdentityServerInteractionService identityServerInteractionService)
		{
			_clientStore = clientStore;
			_resourceStore = resourceStore;
			_identityServerInteractionService = identityServerInteractionService;
		}


		private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
		{
			var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
			if(request == null)
			{
				return null;
			}

			var client = await _clientStore.FindClientByIdAsync(request.ClientId);

			var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

			var vm = CreateConsentViewModel(request, client, resources);
			vm.ReturnUrl = returnUrl;
			return vm;
		}

		private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request,Client client,Resources resources)
		{
			var consentViewModel = new ConsentViewModel();

			consentViewModel.ClientName = client.ClientName;
			consentViewModel.ClientLogoUrl = client.LogoUri;
			consentViewModel.ClientUrl = client.ClientUri;
			//consentViewModel.AllowRememberConsent = client.AllowRememberConsent;
			consentViewModel.RememberConsent = client.AllowRememberConsent;

			consentViewModel.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i));
			consentViewModel.ResourceScopes = resources.ApiResources.SelectMany(a => a.Scopes).Select(s => CreateScopeViewModel(s));

			return consentViewModel;
		}

		private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource)
		{
			return new ScopeViewModel() {
				Name = identityResource.Name,
				DisplayName = identityResource.DisplayName,
				Desription = identityResource.Description,
				Checked  = identityResource.Required,
				Required = identityResource.Required,
				Emphasize = identityResource.Emphasize
			};
		}

		private ScopeViewModel CreateScopeViewModel(Scope scope)
		{
			return new ScopeViewModel()
			{
				Name = scope.Name,
				DisplayName = scope.DisplayName,
				Desription = scope.Description,
				Checked = scope.Required,
				Required = scope.Required,
				Emphasize = scope.Emphasize
			};
		}

		[HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
			var model = await BuildConsentViewModel(returnUrl);
			if(model == null)
			{
				return null;
			}

            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> Index(InputConsentViewModel viewModel)
		{
			ConsentResponse consentResponse=null;
			if(viewModel.Button == "No")
			{
				consentResponse = ConsentResponse.Denied;
			}else if(viewModel.Button == "Yes")
			{
				if(viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
				{
					consentResponse = new ConsentResponse {
						RememberConsent = viewModel.RememberConsent,
						ScopesConsented = viewModel.ScopesConsented,
					};
				}
			}

			if(consentResponse!= null)
			{
				var request =await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
				await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

				return Redirect(viewModel.ReturnUrl);
			}

			return View();
		}
    }
}