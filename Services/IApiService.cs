using Microsoft.BotBuilderSamples;
using Refit;
using System.Threading.Tasks;

namespace CoreBot
{
    public interface IApiService
    {
        [Get("/ws/{renavam}/json")]
        Task<LicenseDialogDetails> GetInfoAsync(string renavam);
    }
}
