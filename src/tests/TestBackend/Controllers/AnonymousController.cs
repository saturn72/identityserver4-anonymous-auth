using System.Threading.Tasks;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.UI.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityServer4.Anonymous.Events;

namespace IdentityServer4.Anonymous.UI.Controllers
{
    [Route("anonymous")]
    [SecurityHeaders]
    public class AnonymousController : Controller
    {
        private readonly IAnonymousCodeStore _codeStore;
        private readonly IAnonymousFlowInteractionService _interaction;
        private readonly IEventService _events;
        private readonly ILogger<AnonymousController> _logger;

        public AnonymousController(
            IAnonymousCodeStore codeStore,
            IAnonymousFlowInteractionService interaction,
            IEventService eventService,
            ILogger<AnonymousController> logger)
        {
            _codeStore = codeStore;
            _interaction = interaction;
            _events = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug($"Start {nameof(Index)}");
            var verificationCode = Request.Query[Constants.UserInteraction.VerificationCode];

            if (string.IsNullOrWhiteSpace(verificationCode))
                return View("BadOrMissingDataError");

            var entry = await _codeStore.FindByVerificationCodeAsync(verificationCode, false);
            if (entry == default)
                return View("BadOrMissingDataError");

            var model = new UserCodeCaptureViewModel
            {
                Transport = entry.Transport,
                VerificationCode = verificationCode,
            };
            var userCode = Request.Query[Constants.UserInteraction.UserCode];
            if (string.IsNullOrWhiteSpace(userCode))
                return View("UserCodeCapture", model);

            model.UserCode = userCode;
            return await ValidateUserCodeCaptureModel(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCodeCapture(UserCodeCaptureViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Error");

            return await ValidateUserCodeCaptureModel(model);
        }
        private async Task<IActionResult> ValidateUserCodeCaptureModel(UserCodeCaptureViewModel model)
        {
            _ = _codeStore.UpdateVerificationRetryAsync(model.VerificationCode);
            var entry = await _codeStore.FindByVerificationCodeAndUserCodeAsync(model.VerificationCode, model.UserCode);
            if (entry == default)
                return View("Error");

            var a = await _interaction.HandleRequestAsync(entry);
            if (a == default || a.IsError)
            {
                await _events.RaiseAsync(new AnonymousGrantFailedEvent(entry));
                return View("Error");
            }
            await _events.RaiseAsync(new AnonymousGrantedEvent(entry));
            return View("Success");
        }
    }
}