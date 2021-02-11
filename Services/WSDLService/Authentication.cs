using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService
{
    public static class Authentication
    {
        public static wsDetranChatBot.autenticacao Auth()
        {
            try
            {
                var auth = new wsDetranChatBot.autenticacao();
                auth.loginUsuario = "0";
                auth.senhaUsuario = "0";

                return auth;
            }
            catch (Exception err)
            {
                LicenseDialogDetails.Error = err.Message;
                return null;
            }
        }
    }
}
