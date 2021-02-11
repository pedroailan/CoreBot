using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.obterEmissaoCRLV
{
    public class ObterEmissaoCRLV
    {
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa, double codSeguranca)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, placa, codSeguranca);

            return soap.obterEmissaoCrlvResult;
        }

        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, placa, 0);

            return soap.obterEmissaoCrlvResult;
        }
    }
}
