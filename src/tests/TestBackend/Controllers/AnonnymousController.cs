using System;
using System.Threading.Tasks;
using IdentityServer4.Anonnymous.Stores;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.UI.Models;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityServer4.Anonnymous.Events;

namespace IdentityServer4.Anonnymous.UI.Controllers
{
    [Route("anonnymous")]
    [SecurityHeaders]
    public class AnonnymousController : Controller
    {
        private readonly IAnnonymousCodeStore _codeStore;
        private readonly IAnonnymousFlowInteractionService _interaction;
        private readonly IEventService _events;
        private readonly ILogger<AnonnymousController> _logger;

        public AnonnymousController(
            IAnnonymousCodeStore codeStore,
            IAnonnymousFlowInteractionService interaction,
            IEventService eventService,
            ILogger<AnonnymousController> logger)
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
                await _events.RaiseAsync(new AnonnymousGrantFailedEvent(entry));
                return View("Error");
            }
            await _events.RaiseAsync(new AnonnymousGrantedEvent(entry));
            return View("Success");
        }
    }
}