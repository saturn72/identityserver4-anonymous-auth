using IdentityModel;
using IdentityServer4.Anonnymous.Services.Generators;
using IdentityServer4.Anonnymous.Stores;
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

namespace IdentityServer4.Anonnymous.Services
{
    public class DefaltAnonnymousFlowInteractionService : IAnonnymousFlowInteractionService
    {
        private readonly IClientStore _clients;
        private readonly IUserSession _session;
        private readonly IAnnonymousCodeStore _codeInfos;
        private readonly IRandomStringGenerator _generator;
        private readonly ILogger<DefaltAnonnymousFlowInteractionService> _logger;

        public DefaltAnonnymousFlowInteractionService(
            IClientStore clients,
            IUserSession session,
            IAnnonymousCodeStore codeInfos,
            IRandomStringGenerator generator,
            ILogger<DefaltAnonnymousFlowInteractionService> logger)
        {
            _clients = clients;
            _session = session;
            _codeInfos = codeInfos;
            _generator = generator;
            _logger = logger;
        }

        public async Task<AnonnymousInteractionResult> HandleRequestAsync(AnonnymousCodeInfo code)
        {
            if (code == default) throw new ArgumentNullException(nameof(code));

            var client = await _clients.FindClientByIdAsync(code.ClientId);
            if (client == default)
                return LogAndReturnError("Invalid client", "Anonnymous authorization failure - requesting client is invalid");

            await CreateSubject(code, client);

            _ = _codeInfos.Authorize(code);
            return new AnonnymousInteractionResult();
        }

        private async Task CreateSubject(AnonnymousCodeInfo code, Client client)
        {
            var existSubjectIds = await _codeInfos.GetAllSubjectIds() ?? Array.Empty<string>();
            do
            {
                code.SubjectId = await _generator.Genetare(8);
            }
            while (existSubjectIds.Contains(code.SubjectId));

            var clientClaims = client.Claims.Select(c => new Claim(c.Type, c.Value));
            var claims = new List<Claim>(clientClaims)
            {
                new Claim(JwtClaimTypes.Subject, code.SubjectId)
            };
            var ci = new ClaimsIdentity(claims, Constants.AnonnymousAuthenticationType);
            var subject = new ClaimsPrincipal(ci);
            code.SessionId = await _session.CreateSessionIdAsync(subject, new AuthenticationProperties());
            code.AuthorizedScopes = client.AllowedScopes.Intersect(code.RequestedScopes);
        }

        private AnonnymousInteractionResult LogAndReturnError(string error, string errorDescription = null)
        {
            _logger.LogError(errorDescription);
            return AnonnymousInteractionResult.Failure(error);
        }
    }
    public record AnonnymousInteractionResult
    {
        public bool IsError { get; set; }
        public string ErrorDescription { get; set; }

        public static AnonnymousInteractionResult Failure(string errorDescription = null)
        {
            return new AnonnymousInteractionResult
            {
                IsError = true,
                ErrorDescription = errorDescription
            };
        }
    }
}
