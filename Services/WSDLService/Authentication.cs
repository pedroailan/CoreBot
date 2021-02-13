using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService
{
    public static class Authentication
    {
        public static wsDetranChatBot.wsChatbotSoapClient WsClient()
        {
            try
            {
                wsDetranChatBot.wsChatbotSoapClient wsClient = new wsDetranChatBot.wsChatbotSoapClient(new wsDetranChatBot.wsChatbotSoapClient.EndpointConfiguration(), "http://172.28.64.58:9176/homologa/serviceChatBot");
                return wsClient;
            }
            catch (Exception err)
            {
                //LicenseDialogDetails.Error = err.Message;
                return null;
            }
        }

        public static wsDetranChatBot.autenticacao Auth()
        {
            try
            {
                wsDetranChatBot.autenticacao auth = new wsDetranChatBot.autenticacao();
                auth.loginUsuario = "0";
                auth.senhaUsuario = "0";
                return auth;
            }
            catch (Exception err)
            {
                //LicenseDialogDetails.Error = err.Message;
                return null;
            }
        }
    }
}
