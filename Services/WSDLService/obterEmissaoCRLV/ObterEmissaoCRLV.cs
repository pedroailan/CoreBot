using CoreBot.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.obterEmissaoCRLV
{
    
    public class ObterEmissaoCRLV
    {
        /// <summary>
        /// OBJETIVO(S): 
        ///     Autenticar o cliente com a interface no WebService,
        ///     Obter as informações de forma assíncrona.
        ///     OBS: Os construtores possuem sobrecarga.
        /// AUTOR(RES): Felipe Falcão e Pedro Ailan
        /// </summary>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa, double codSeguranca)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, placa, codSeguranca);
            var result = soap.obterEmissaoCrlvResult;

            History(result);

            return soap.obterEmissaoCrlvResult;
        }

        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, placa, 0);
            var result = soap.obterEmissaoCrlvResult;

            History(result);

            return soap.obterEmissaoCrlvResult;
        }

        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(double codSeguranca)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, "OEK8190", codSeguranca);
            var result = soap.obterEmissaoCrlvResult;

            History(result);

            return soap.obterEmissaoCrlvResult;
        }

        /// <summary>
        /// OBJETIVO(S): 
        ///     Preencher as Fields da classe CrlvDialogsDetails com os atributos recebidos.
        /// AUTOR(RES): Felipe Falcão e Pedro Ailan
        /// </summary>
        public void History(wsDetranChatBot.obterEmissaoCrlvResult result)
        {
            CRLVDialogDetails.codigoRetorno = result.codigoRetorno;
            CRLVDialogDetails.Erro.codigo = result.erro.codigo;
            CRLVDialogDetails.Erro.mensagem = result.erro.mensagem;
            CRLVDialogDetails.Erro.trace = result.erro.trace;
            CRLVDialogDetails.codSegurançaOut = result.codSegurancaOut.ToString();
            CRLVDialogDetails.renavam = result.renavam.ToString();
            CRLVDialogDetails.placaOut = result.placaOut;
            CRLVDialogDetails.nomeProprietario = result.nomeProprietario;
            CRLVDialogDetails.documentoCRLVePdf = result.documentoCRLVePdf;
        }
    }
}
