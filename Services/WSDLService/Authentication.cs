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
                LicenseDialogDetails.ErrorService = err.Message;
                return null;
            }
        }

        public static wsDetranChatBot.autenticacao Auth()
        {
            try
            {
                wsDetranChatBot.autenticacao auth = new wsDetranChatBot.autenticacao
                {
                    loginUsuario = "4030852F-26A1-4BA7-A4E0-30940E210CF3",
                    senhaUsuario = "bfce160d0941496f935ea762806c9160"
                };
                return auth;
            }
            catch (Exception err)
            {
                LicenseDialogDetails.ErrorAuthentication = err.Message;
                return null;
            }
        }
    }
}
