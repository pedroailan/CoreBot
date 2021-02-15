using Microsoft.BotBuilderSamples;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.ServiceLicenciamento
{
    interface IApiServiceVD
    {
        [Get("/ws/{renavam}/json")]
        Task<LicenseDialogDetails> VServiceLicenciamento(int codSeguranca, int renavam, string tipoDocumentoIn, int anoLicenciamentoIn);
    }
}
