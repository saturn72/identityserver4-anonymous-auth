using System;
using System.Threading.Tasks;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.UI.Models;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Anonnymous.UI.Controllers
{
    [Route("anonnymous")]
    [SecurityHeaders]
    public class AnonnymousController : Controller
    {
        private readonly IAnonnymousCodeService _anonnymousCodeService;
        //      private readonly IAnonnymousFlowInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IOptions<IdentityServerOptions> _options;
        private readonly ILogger<AnonnymousController> _logger;

        public AnonnymousController(
            IAnonnymousCodeService anonnymousCodeService,
            //IAnonnymousFlowInteractionService interaction,
            IEventService eventService,
            IOptions<IdentityServerOptions> options,
            ILogger<AnonnymousController> logger)
        {
            _anonnymousCodeService = anonnymousCodeService;
            //   _interaction = interaction;
            _events = eventService;
            _options = options;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug($"Start {nameof(Index)}");
            var verificationCode = Request.Query[Constants.UserInteraction.VerificationCode];

            if (string.IsNullOrWhiteSpace(verificationCode))
                return View("BadOrMissingDataError");

            var entry = await _anonnymousCodeService.FindByVerificationCodeAsync(verificationCode);
            if (entry == default)
                return View("BadOrMissingDataError");

            
            var userCode = Request.Query[Constants.UserInteraction.UserCode];
            if (string.IsNullOrWhiteSpace(userCode))
            {
                var model = new UserCodeCaptureViewModel
                {
                    Transport = entry.Transport,
                    VerificationCode = verificationCode,
                };
                return View("UserCodeCapture", model);
            }

            throw new NotImplementedException();

            /*



            return View("Error");

            //if both exists and valid - show "success"


            return await HandleErrorResultAsync($"fail to locate wntry with anonnymous_code: {verificationCode}", "bad or missing data");

            if (string.IsNullOrWhiteSpace(verificationCode)) return View("UserCodeCapture", new { showVerificationField = true });

            var entry = await _anonnymousCodeService.FindByVerificationCodeAsync(verificationCode);
            if (entry == default)
                return await HandleErrorResultAsync($"fail to locate wntry with anonnymous_code: {verificationCode}", "bad or missing data");


            var vm = await BuildViewModelAsync(userCode);
            if (vm == null) return View("Error");

            vm.ConfirmUserCode = true;
            return View("UserCodeConfirmation", vm);

            /* 



            // generate anonnymous_code
            var client = await _clientStore.FindClientByIdAsync(entry.ClientId);
            if (client == default)
                return await HandleErrorResultAsync($"fail to locate client with id: {entry.ClientId}", "bad or missing data");

            var code = await GenerateUserCodeAsync(client.UserCodeType ?? _options.DefaultUserCodeType);
            _ = _anonnymousCodeService.Activate(entry.Id, code);
            var ctx = new AnonnymousCodeTransportContext
            {
                Transport = entry.Transport,
                Provider = entry.TransportProvider,
                Data = entry.TransportData,
            };
            ctx.Body = await BuildMessageBody(client, code, ctx);
            _ = _transports.Transport(ctx);

            return new StatusCodeResult(HttpStatusCode.OK);/
        }

       }*/
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCodeCapture(UserCodeCaptureViewModel model)
        {
            _ = _anonnymousCodeService.UpdateVerificationRetryAsync(model.VerificationCode);
            var entry = await _anonnymousCodeService.FindByVerificationCodeAndUserCodeAsync(model.VerificationCode, model.UserCode);
            if(entry == default)
                return View("UserCodeCapture", model);
            throw new NotImplementedException();
            //capture.UserCode
            //    var vm = await BuildViewModelAsync(userCode);
            //if (vm == null) return View("Error");

            //return View("UserCodeConfirmation", capture);
        }
        /*

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Callback(AnonnymousAuthorizationInputModel model)
    //{
    //    if (model == null) throw new ArgumentNullException(nameof(model));

    //    var result = await ProcessConsent(model);
    //    if (result.HasValidationError) return View("Error");

    //    return View("Success");
    //}

    //private async Task<ProcessConsentResult> ProcessConsent(AnonnymousAuthorizationInputModel model)
    //{
    //    var result = new ProcessConsentResult();

    //    var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
    //    if (request == null) return result;

    //    ConsentResponse grantedConsent = null;

    //    // user clicked 'no' - send back the standard 'access_denied' response
    //    if (model.Button == "no")
    //    {
    //        grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

    //        // emit event
    //        await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
    //    }
    //    // user clicked 'yes' - validate the data
    //    else if (model.Button == "yes")
    //    {
    //        // if the user consented to some scope, build the response model
    //        if (model.ScopesConsented != null && model.ScopesConsented.Any())
    //        {
    //            var scopes = model.ScopesConsented;
    //            if (ConsentOptions.EnableOfflineAccess == false)
    //            {
    //                scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
    //            }

    //            grantedConsent = new ConsentResponse
    //            {
    //                RememberConsent = model.RememberConsent,
    //                ScopesValuesConsented = scopes.ToArray(),
    //                Description = model.Description
    //            };

    //            // emit event
    //            await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
    //        }
    //        else
    //        {
    //            result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
    //        }
    //    }
    //    else
    //    {
    //        result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
    //    }

    //    if (grantedConsent != null)
    //    {
    //        // communicate outcome of consent back to identityserver
    //        await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);

    //        // indicate that's it ok to redirect back to authorization endpoint
    //        result.RedirectUri = model.ReturnUrl;
    //        result.Client = request.Client;
    //    }
    //    else
    //    {
    //        // we need to redisplay the consent UI
    //        result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
    //    }

    //    return result;
    //}

    //private async Task<AnonnymousAuthorizationViewModel> BuildViewModelAsync(string userCode, AnonnymousAuthorizationInputModel model = null)
    //{
    //    var request = await _interaction.GetAuthorizationContextAsync(userCode);
    //    if (request != null)
    //    {
    //        return CreateConsentViewModel(userCode, model, request);
    //    }

    //    return null;
    //}

    //private AnonnymousAuthorizationViewModel CreateConsentViewModel(string userCode, AnonnymousAuthorizationInputModel model, AnonnymousFlowAuthorizationRequest request)
    //{
    //    var vm = new AnonnymousAuthorizationViewModel
    //    {
    //        UserCode = userCode,
    //        Description = model?.Description,

    //        RememberConsent = model?.RememberConsent ?? true,
    //        ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),

    //        ClientName = request.Client.ClientName ?? request.Client.ClientId,
    //        ClientUrl = request.Client.ClientUri,
    //        ClientLogoUrl = request.Client.LogoUri,
    //        AllowRememberConsent = request.Client.AllowRememberConsent
    //    };

    //    vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

    //    var apiScopes = new List<ScopeViewModel>();
    //    foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
    //    {
    //        var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
    //        if (apiScope != null)
    //        {
    //            var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
    //            apiScopes.Add(scopeVm);
    //        }
    //    }
    //    if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
    //    {
    //        apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
    //    }
    //    vm.ApiScopes = apiScopes;

    //    return vm;
    //}

    //private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    //{
    //    return new ScopeViewModel
    //    {
    //        Value = identity.Name,
    //        DisplayName = identity.DisplayName ?? identity.Name,
    //        Description = identity.Description,
    //        Emphasize = identity.Emphasize,
    //        Required = identity.Required,
    //        Checked = check || identity.Required
    //    };
    //}

    //public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    //{
    //    return new ScopeViewModel
    //    {
    //        Value = parsedScopeValue.RawValue,
    //        // todo: use the parsed scope value in the display?
    //        DisplayName = apiScope.DisplayName ?? apiScope.Name,
    //        Description = apiScope.Description,
    //        Emphasize = apiScope.Emphasize,
    //        Required = apiScope.Required,
    //        Checked = check || apiScope.Required
    //    };
    //}
    //private ScopeViewModel GetOfflineAccessScope(bool check)
    //{
    //    return new ScopeViewModel
    //    {
    //        Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
    //        DisplayName = ConsentOptions.OfflineAccessDisplayName,
    //        Description = ConsentOptions.OfflineAccessDescription,
    //        Emphasize = true,
    //        Checked = check
    //    };
    //}*/
    }
}