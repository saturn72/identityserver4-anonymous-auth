using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface IAnonnymousFlowInteractionService
    {
        Task<AnonnymousInteractionResult> HandleRequestAsync(AnonnymousCodeInfo codeInfo);
    }
}
