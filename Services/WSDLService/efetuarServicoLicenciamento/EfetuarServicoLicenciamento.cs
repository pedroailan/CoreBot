using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.efetuarServicoLicenciamento
{
    public class EfetuarServicoLicenciamento
    {

        public static async Task<wsDetranChatBot.efetuarServicoLicenciamentoResult> efeutarServicoLicenciamento(
            double renavam, 
            double codSeguranca, 
            string restricao, 
            double anoExercicioLicenciamento, 
            string tipoAutorizacaoRNTRC, 
            double nroAutorizacaoRNTRC, 
            string dataValidadeRNTRC, 
            string isencaoIPVA, 
            string tipoDocumentoIn
            )
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.efetuarServicoLicenciamentoAsync(
                auth, 
                renavam, 
                codSeguranca, 
                restricao, 
                anoExercicioLicenciamento, 
                tipoAutorizacaoRNTRC, 
                nroAutorizacaoRNTRC, 
                dataValidadeRNTRC, 
                isencaoIPVA, 
                tipoDocumentoIn
                );

            var result = soap.efetuarServicoLicenciamentoResult;

            alocarFields(result);

            return soap.efetuarServicoLicenciamentoResult;
        }


        public static void alocarFields(wsDetranChatBot.efetuarServicoLicenciamentoResult result)
        {
            LicenseDialogDetails.codigoRetorno = result.codigoRetorno;
            LicenseDialogDetails.Erro.codigo = result.erro.codigo;
            LicenseDialogDetails.Erro.mensagem = result.erro.mensagem;
            LicenseDialogDetails.Erro.trace = result.erro.trace;
            LicenseDialogDetails.placa = result.placa;


        }
    }
}
