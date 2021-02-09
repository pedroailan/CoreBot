using CoreBot.Services.Fields;
using CoreBot.Services.ServiceLicenciamento;
using Microsoft.BotBuilderSamples;
using Refit;
using System;
using System.Threading.Tasks;

namespace CoreBot.Services.Models
{
    public class Consumer
    {
        public static async Task ConsumerAsync()
        {
            var inFields = new InFields();
            try
            {
                var codRenavam = RestService.For<IApiService>("http://172.28.64.58:8176/wsChatbot?wsdl");

                var info = await codRenavam.ServiceLicenciamento(inFields.codSeguranca, inFields.renavam, inFields.restricao, inFields.exercicioLicenciamento, inFields.tipoAutorizacaoRNTRC, inFields.dataValidadeRNTRC, inFields.nroAutorizacao, inFields.isencaoIPVA, inFields.tipoDocumentoIn);

            }
            catch (Exception e)
            {

                LicenseDialogDetails.Error = e.Message;
            }

        }
    }
}
