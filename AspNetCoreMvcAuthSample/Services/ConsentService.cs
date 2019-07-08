using AspNetCoreMvcAuthSample.ViewModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample.Services
{
	public class ConsentService
	{
		private readonly IClientStore _clientStore;

		private readonly IResourceStore _resourceStore;

		private readonly IIdentityServerInteractionService _identityServerInteractionService;

		public ConsentService(IClientStore clientStore
			, IResourceStore resourceStore
			, IIdentityServerInteractionService identityServerInteractionService)
		{
			_clientStore = clientStore;
			_resourceStore = resourceStore;
			_identityServerInteractionService = identityServerInteractionService;
		}

		public async Task<ConsentViewModel> BuildConsentViewModelAsync(string returnUrl,InputConsentViewModel model =null)
		{
			var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
			if (request == null)
			{
				return null;
			}

			var client = await _clientStore.FindClientByIdAsync(request.ClientId);

			var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

			var vm = CreateConsentViewModel(request, client, resources,model);
			vm.ReturnUrl = returnUrl;
			return vm;
		}

		public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel viewModel)
		{
			ConsentResponse consentResponse = null;
			var result = new ProcessConsentResult();
			if (viewModel.Button == "No")
			{
				consentResponse = ConsentResponse.Denied;
			}
			else if (viewModel.Button == "Yes")
			{
				if (viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
				{
					consentResponse = new ConsentResponse
					{
						RememberConsent = viewModel.RememberConsent,
						ScopesConsented = viewModel.ScopesConsented,
					};
				}
				else
				{
					result.ValidationError = "请至少选中一个权限";
				}
			}

			if (consentResponse != null)
			{
				var request = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
				await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

				result.RedireUrl = viewModel.ReturnUrl;
			}

			var consentViewModel = await BuildConsentViewModelAsync(viewModel.ReturnUrl,viewModel);
			result.ViewModel = consentViewModel;

			return result;
		}

		#region Private Method
		private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources,InputConsentViewModel model)
		{
			var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();

			var consentViewModel = new ConsentViewModel();

			consentViewModel.ClientName = client.ClientName;
			consentViewModel.ClientLogoUrl = client.LogoUri;
			consentViewModel.ClientUrl = client.ClientUri;
			//consentViewModel.AllowRememberConsent = client.AllowRememberConsent;
			consentViewModel.RememberConsent = (model == null || model.RememberConsent);// client.AllowRememberConsent;

			consentViewModel.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i,model == null || selectedScopes.Contains(i.Name)));
			consentViewModel.ResourceScopes = resources.ApiResources.SelectMany(a => a.Scopes).Select(s => CreateScopeViewModel(s, model == null || selectedScopes.Contains(s.Name)));

			return consentViewModel;
		}

		private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource,bool check)
		{
			return new ScopeViewModel()
			{
				Name = identityResource.Name,
				DisplayName = identityResource.DisplayName,
				Desription = identityResource.Description,
				Checked = check || identityResource.Required,
				Required = identityResource.Required,
				Emphasize = identityResource.Emphasize
			};
		}

		private ScopeViewModel CreateScopeViewModel(Scope scope,bool check)
		{
			return new ScopeViewModel()
			{
				Name = scope.Name,
				DisplayName = scope.DisplayName,
				Desription = scope.Description,
				Checked = check || scope.Required,
				Required = scope.Required,
				Emphasize = scope.Emphasize
			};
		} 
		#endregion

	}
}
