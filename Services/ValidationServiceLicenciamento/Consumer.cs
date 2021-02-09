using CoreBot.Services.Fields;
using CoreBot.Services.ServiceLicenciamento;
using Microsoft.BotBuilderSamples;
using Refit;
using System;
using System.Threading.Tasks;

namespace CoreBot.Services.ValidationServiceLicenciamento
{
    public class Consumer
    {
        public static async Task ConsumerAsync()
        {
            var inFields = new InFields();
            try
            {
                var codRenavam = RestService.For<IApiServiceVD>("http://172.28.64.58:8176/wsChatbot?wsdl");

                var info = await codRenavam.VServiceLicenciamento(inFields.codSeguranca, inFields.renavam, inFields.tipoDocumentoIn, inFields.anoLicenciamento);
            }
            catch (Exception e)
            {
                LicenseDialogDetails.Error = e.Message;
            }
        }
    }
}
