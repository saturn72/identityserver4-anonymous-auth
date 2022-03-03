using IdentityModel;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services
{
    public class DefaltAnonymousFlowInteractionService : IAnonymousFlowInteractionService
    {
        private readonly IClientStore _clients;
        private readonly IUserSession _session;
        private readonly IAnonymousCodeStore _codeInfos;
        private readonly IRandomStringGenerator _generator;
        private readonly ILogger<DefaltAnonymousFlowInteractionService> _logger;

        public DefaltAnonymousFlowInteractionService(
            IClientStore clients,
            IUserSession session,
            IAnonymousCodeStore codeInfos,
            IRandomStringGenerator generator,
            ILogger<DefaltAnonymousFlowInteractionService> logger)
        {
            _clients = clients;
            _session = session;
            _codeInfos = codeInfos;
            _generator = generator;
            _logger = logger;
        }

        public async Task<AnonymousInteractionResult> HandleRequestAsync(AnonymousCodeInfo code)
        {
            if (code == default)
                return LogAndReturnError("Invalid request", $"Anonymous authorization failure - {nameof(code)} is missing");

            var client = await _clients.FindClientByIdAsync(code.ClientId);
            if (client == default)
                return LogAndReturnError("Invalid client", "Anonymous authorization failure - requested client is invalid");

            await UpdateForAuthorization(code, client);

            _ = _codeInfos.PrepareForAuthorizationUpdate(code);
            return new AnonymousInteractionResult();
        }

        private async Task UpdateForAuthorization(AnonymousCodeInfo code, Client client)
        {
            string subject = "";
            if (!code.Id.Equals(default))
                subject = code.Id.ToString("N");
            var existSubjectIds = await _codeInfos.GetAllSubjectIdsByClientIdAsync(client.ClientId) ?? Array.Empty<string>();

            while (existSubjectIds.Contains(subject) || !subject.HasValue())
            {
                subject = await _generator.Genetare(32);
            }
            var clientClaims = client.Claims.Select(c => new Claim(c.Type, c.Value));
            var claims = new List<Claim>(clientClaims)
            {
                new Claim(JwtClaimTypes.Subject, subject)
            };
            var ci = new ClaimsIdentity(claims, Constants.AnonymousAuthenticationType);
            var principal = new ClaimsPrincipal(ci);

            code.Subject = subject;
            code.SessionId = await _session.CreateSessionIdAsync(principal, new AuthenticationProperties());
            code.Claims = claims;
            code.AuthorizedScopes = client.AllowedScopes.Intersect(code.RequestedScopes);
        }

        private AnonymousInteractionResult LogAndReturnError(string error, string errorDescription = null)
        {
            _logger.LogError(errorDescription);
            return AnonymousInteractionResult.Failure(error);
        }
    }
    public record AnonymousInteractionResult
    {
        public bool IsError { get; set; }
        public string ErrorDescription { get; set; }

        public static AnonymousInteractionResult Failure(string errorDescription = null)
        {
            return new AnonymousInteractionResult
            {
                IsError = true,
                ErrorDescription = errorDescription
            };
        }
    }
}
