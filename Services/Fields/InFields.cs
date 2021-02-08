using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.Fields
{
    public class InFields
    {
        LicenseDialogDetails LicenseDialogDetails;

        public int renavam = Convert.ToInt32(LicenseDialogDetails.Renavam);
        public int codSeguranca = Convert.ToInt32(LicenseDialogDetails.SecureCode);
        public string restricao; //será enviado com base no retorno da validação
        public int exercicioLicenciamento = Convert.ToInt32(LicenseDialogDetails.AnoExercicio);
        public string tipoAutorizacaoRNTRC = LicenseDialogDetails.TipoDeAutorização;
        public string nroAutorizacao = LicenseDialogDetails.NumeroDeAutorizacao;
        public string dataValidadeRNTRC = LicenseDialogDetails.DataDeAutorizacao;
        public string isencaoIPVA = LicenseDialogDetails.IsencaoIPVA;
        public string tipoDocumentoIn = LicenseDialogDetails.TipoDocumento;
        public int anoLicenciamento = Convert.ToInt32(LicenseDialogDetails.AnoExercicio);
    }
}
