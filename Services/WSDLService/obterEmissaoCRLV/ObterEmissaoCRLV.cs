using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.obterEmissaoCRLV
{
    public class ObterEmissaoCRLV
    {
        public wsDetranChatBot.obterEmissaoCrlvResponse obterEmissaoCRLV(wsDetranChatBot.autenticacao auth, string placa, double codSeguranca)
        {
            var consulta = new wsDetranChatBot.obterEmissaoCrlvRequest(auth, placa, codSeguranca);
            var result = new wsDetranChatBot.obterEmissaoCrlvResult();
            var response = new wsDetranChatBot.obterEmissaoCrlvResponse();

            return response;
        }
    }
}
