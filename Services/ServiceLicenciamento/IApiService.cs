using Microsoft.BotBuilderSamples;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.ServiceLicenciamento
{
    interface IApiService
    {
        [Get("/ws/{renavam}/json")]
        Task<LicenseDialogDetails> ServiceLicenciamento(int codSeguranca, int renavam, string restricao, int exercicio, string tipoAutorizacaoRNTRC, string nroAutorizacaoRNTRC, string dataValidadeRNTRC, string isencaoIPVA, string tipoDocumentoIn);
    }
}
