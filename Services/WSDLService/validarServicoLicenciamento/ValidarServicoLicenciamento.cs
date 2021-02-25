using CoreBot.Services.Fields;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.validarServicoLicenciamento
{
    public class ValidarServicoLicenciamento
    {
        LicenseDialogDetails dados;
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> validarServicoLicenciamento(double renavam, double codSeguranca, string tipoDocumentoIn, double anoLicenciamentoIn)
        {
            
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();
             
            var soap = await wsClient.validarServicoLicenciamentoAsync(auth, renavam, codSeguranca, tipoDocumentoIn, anoLicenciamentoIn);
            var result = soap.validarServicoLicenciamentoResult;

           //History(result);

            return result;
        }


        public void History(wsDetranChatBot.validarServicoLicenciamentoResult result)
        {
            LicenseDialogDetails dados1 = new LicenseDialogDetails();
            LicenseDialogDetails.codigoRetorno = result.codigoRetorno;
            LicenseDialogDetails.Erro.codigo = result.erro.codigo;
            LicenseDialogDetails.Erro.mensagem = result.erro.mensagem;
            LicenseDialogDetails.Erro.trace = result.erro.trace;
            LicenseDialogDetails.codSegurancaOut = result.codSegurancaOut.ToString();
            LicenseDialogDetails.renavamOut = result.renavamOut.ToString();
            LicenseDialogDetails.placa = result.placa;
            dados1.nomeProprietario = result.nomeProprietario;
            LicenseDialogDetails.temRNTRC = result.temRNTRC;
            LicenseDialogDetails.tipoAutorizacaoRNTRCOut = result.tipoAutorizacaoRNTRC;
            LicenseDialogDetails.nroAutorizacaoRNTRCOut = result.nroAutorizacaoRNTRC;
            LicenseDialogDetails.temIsençãoIPVA = result.temIsencaoIPVA;
            LicenseDialogDetails.restricao = result.restricao;
            LicenseDialogDetails.anoLicenciamento = result.anoLicenciamento;
            LicenseDialogDetails.totalCotaUnica = result.totalCotaUnica;
            LicenseDialogDetails.contadorAnoLicenciamento = result.contadorAnoLicenciamento;
            LicenseDialogDetails.RecallPendente.codigo = result.recallPendente.codigo;
            LicenseDialogDetails.RecallPendente.mensagem = result.recallPendente.mensagem;
            LicenseDialogDetails.RecallPendente.ListaRecall.descricao = new string[] { result.recallPendente.listaRecall.ToString() };
        }
    }
}
