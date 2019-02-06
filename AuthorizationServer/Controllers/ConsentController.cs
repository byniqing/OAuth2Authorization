using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using IdentityServer4.Models;
using AuthorizationServer.ViewModels;

namespace AuthorizationServer.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<ConsentController> _logger;

        public ConsentController(IClientStore clientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService interaction,
             ILogger<ConsentController> logger)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _interaction = interaction;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("index", vm);
            }
            return View("Error");
        }
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            // 验证URL是否有效
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null) return result;
            ConsentResponse grantedConsent = null;
            if (model.Button == "no")
            {
                grantedConsent = ConsentResponse.Denied;
            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes.ToArray()
                    };
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }
            if (grantedConsent != null)
            {
                // 把同意的结果发送给 identityserver
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                //result.RedirectUri = "http://localhost:5001/";
                result.ClientId = request.ClientId;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
            }
            return result;
        }
        [HttpPost]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await ProcessConsent(model);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }
            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }

        public async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            ConsentViewModel cvm = null;
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if (request != null)
            {
                var client = await _clientStore.FindClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    cvm = new ConsentViewModel
                    {
                        ClientName = client.ClientName,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        AllowRememberConsent = client.AllowRememberConsent,
                        ReturnUrl = returnUrl,
                        RememberConsent = model?.RememberConsent ?? true,
                        ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                    };

                    var resources = await _resourceStore.FindResourcesByScopeAsync(request.ScopesRequested);

                    var apiResource = await _resourceStore.FindApiResourcesByScopeAsync(request.ScopesRequested);
                    var identitResource = await _resourceStore.FindIdentityResourcesByScopeAsync(request.ScopesRequested);

                    if (apiResource != null && apiResource.Any())
                    {
                        cvm.ResourceScopes = apiResource.SelectMany(i => i.Scopes)
                            .Select(s => CreateScopeViewModel(s, cvm.ScopesConsented.Contains(s.Name) || model == null));
                    }

                    if (identitResource != null && identitResource.Any())
                    {
                        cvm.IdentityScopes = identitResource.Select(s => CreateScopeViewModel(s, cvm.ScopesConsented.Contains(s.Name) || model == null));
                    }

                    if (resources != null && (resources.ApiResources.Any() || resources.IdentityResources.Any()))
                    {

                    }
                }
            }
            return cvm;
        }
        private ScopeViewModel CreateScopeViewModel(IdentityResource scope, bool check)
        {
            return new ScopeViewModel
            {
                Checked = check || scope.Required,
                Required = scope.Required,
                Description = scope.Description,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Name = scope.Name
            };
        }
        private ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Checked = check || scope.Required,
                Required = scope.Required,
                Description = scope.Description,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Name = scope.Name
            };
        }
    }
}