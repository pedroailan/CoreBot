using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.validarServicoLicenciamento;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class VehicleLicense
    {
        public async static Task<bool> ValidationSecureCodeLicenciamento(string SecureCode)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat(SecureCode) == true)
            {
                //16736005660
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var crlv = await obter.validarServicoLicenciamento(499837630, Convert.ToDouble(SecureCode), "D", 2020);
                if (LicenseDialogDetails.Erro.codigo == 0)
                {
                    return true;
                }
            }
            return false;
            //if (Api.LerArquivoJson("codigodeseguranca", SecureCode) == true)
            //{
            //    return true;
            //}
        }

        public static bool ExistSecureCode()
        {
            if (Convert.ToDouble(LicenseDialogDetails.codSegurancaOut) > 0)
            {
                LicenseDialogDetails.secureCodeBool = true;
                return true;
            }
            return false;
        }

        public async static Task<bool> ValidationRenavam(string Renavam)
        {
            if (Renavam.Length > 0 && Format.Input.ValidationFormat(Renavam) == true)
            {
                //16736005660
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var crlv = await obter.validarServicoLicenciamento(Convert.ToDouble(Renavam), 0, "D", 2020);
                if (LicenseDialogDetails.Erro.codigo == 0)
                {
                    return true;
                }
            }
            return false;
            //if (Renavam == "1234") return true;
            //return false;
        }

        public static bool Pendency()
        {
            if (LicenseDialogDetails.anoLicenciamento != null)
            {
                return true;
            }
            return false;
        }

        public static bool Situation(string placa)
        {
            return true;
        }

        public static bool ValidationYear()
        {
            if(LicenseDialogDetails.contadorAnoLicenciamento == 1)
            {
                return true;        
            }
            return false;
        }
    }

       
}
