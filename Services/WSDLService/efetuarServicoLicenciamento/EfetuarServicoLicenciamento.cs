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

            //History(result);

            return soap.efetuarServicoLicenciamentoResult;
        }


        public static void History(wsDetranChatBot.efetuarServicoLicenciamentoResult result)
        {
            LicenseDialogDetails.codigoRetorno = result.codigoRetorno;
            LicenseDialogDetails.Erro.codigo = result.erro.codigo;
            LicenseDialogDetails.Erro.mensagem = result.erro.mensagem;
            LicenseDialogDetails.Erro.trace = result.erro.trace;
            LicenseDialogDetails.cpfProcurador = result.cpfProcurador;
            LicenseDialogDetails.numeroDocumento = result.numeroDocumento;
            LicenseDialogDetails.tipoDocumentoOut = result.tipoDocumento;
            LicenseDialogDetails.cor = result.cor;
            LicenseDialogDetails.vetTaxas = result.vetTaxas;
            LicenseDialogDetails.vetDescDebitos = result.vetDescDebitos;
            LicenseDialogDetails.dataProcessamento = result.dataProcessamento;
            LicenseDialogDetails.exercicio = result.exercicio;
            LicenseDialogDetails.ind = result.ind;
            LicenseDialogDetails.marcaModelo = result.marcaModelo;
            LicenseDialogDetails.nome = result.nome;
            LicenseDialogDetails.placa = result.placa;
            LicenseDialogDetails.renavamOut = result.renavam.ToString();
            LicenseDialogDetails.tipo = result.tipo;
            LicenseDialogDetails.vetValorA = result.vetValorA;
            LicenseDialogDetails.valorApagar = result.valorApagar;
            LicenseDialogDetails.vencimento = result.vencimento;
            LicenseDialogDetails.agencia = result.agencia;
            LicenseDialogDetails.mensagem1 = result.mensagem1;
            LicenseDialogDetails.mensagem2 = result.mensagem2;
            LicenseDialogDetails.mensagem3 = result.mensagem3;
            LicenseDialogDetails.mensagem4 = result.mensagem4;
            LicenseDialogDetails.mensagem5 = result.mensagem5;
            LicenseDialogDetails.totalA = result.mensagem5;
            LicenseDialogDetails.linhaDig = result.linhaDig;
            LicenseDialogDetails.linhaCodBarra = result.linhaCodBarra;
            LicenseDialogDetails.codBarra = result.codBarra;
            LicenseDialogDetails.asBace1 = result.asBace1;
            LicenseDialogDetails.indDescricao = result.indDescricao;
            LicenseDialogDetails.vetDescInfracao = result.vetDescInfracao;
            LicenseDialogDetails.indMensagem = result.indMensagem;
            LicenseDialogDetails.vetDuaMensagem = result.vetDuaMensagem;
            LicenseDialogDetails.chassiSNG = result.chassiSNG;
            LicenseDialogDetails.tituloVenc = result.tituloVenc;
            LicenseDialogDetails.datsVenc = result.datsVenc;
            LicenseDialogDetails.indParc = result.indParc;
            LicenseDialogDetails.vetDuaParc = result.vetDuaParc;
            LicenseDialogDetails.vetValorA1Parc = result.vetValorA1Parc;
            LicenseDialogDetails.vetLinhaDigParc = result.vetLinhaDigParc;
            LicenseDialogDetails.vetLinhaCodBarra = result.vetLinhaCodBarra;
            LicenseDialogDetails.vetCodBarraParc = result.vetCodBarraParc;
            LicenseDialogDetails.vetASBACE1Parc = result.vetASBACE1Parc;
            LicenseDialogDetails.vetValorA2Parc = result.vetValorA2Parc;
            LicenseDialogDetails.vetValorA3Parc = result.vetValorA3Parc;
            LicenseDialogDetails.vetTotalAParc = result.vetTotalAParc;
            LicenseDialogDetails.vetVencimentoParc = result.vetVencimentoParc;
            LicenseDialogDetails.flagParc1A = result.flagParc1A;
            LicenseDialogDetails.flagParc2A = result.flagParc2A;
            LicenseDialogDetails.flagParc3A = result.flagParc3A;
            LicenseDialogDetails.cpfCnpjPagador = result.cpfCnpjPagador;
            LicenseDialogDetails.enderecoPagador = result.enderecoPagador;
            LicenseDialogDetails.cepPagador = result.cepPagador;
            LicenseDialogDetails.bairroPagador = result.bairroPagador;
            LicenseDialogDetails.municipioPagador = result.municipioPagador;
            LicenseDialogDetails.ufPagador = result.ufPagador;
            LicenseDialogDetails.nossoNumero = result.nossoNumero;
        }
    }
}
