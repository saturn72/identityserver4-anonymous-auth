using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services
{
    public interface IAnonymousFlowInteractionService
    {
        Task<AnonymousInteractionResult> HandleRequestAsync(AnonymousCodeInfo codeInfo);
    }
}
