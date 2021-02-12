using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.Fields
{
    
    public class InFields
    {
        static LicenseDialogDetails LicenseDialogDetails;

        public int renavam;
        public int codSeguranca;
        public string restricao; //será enviado com base no retorno da validação
        public int exercicioLicenciamento;
        public string tipoAutorizacaoRNTRC;
        public string nroAutorizacao;
        public string dataValidadeRNTRC;
        public string isencaoIPVA;
        public string tipoDocumentoIn;
        public int anoLicenciamento;
    }
}
