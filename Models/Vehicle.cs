using CoreBot.Fields;
using CoreBot.Services.WSDLService.obterEmissaoCRLV;
using CoreBot.Services.WSDLService.validarServicoLicenciamento;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class Vehicle
    {
        public static bool ValidationVehicleType()
        {
            if (Api.BuscaJson("rtrc") == true)
            {
                return true;
            }
            return false;
        }

        public async static Task<bool> ValidationSecureCode(string SecureCode)
        {
            if (SecureCode.Length > 0 && ValidatonFormat(SecureCode) == true)
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var crlv = await obter.obterEmissaoCRLV(Convert.ToDouble(SecureCode));
                if (CRLVDialogDetails.Erro.codigo == 0)
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
        public async static Task<bool> ValidationSecureCodeLicenciamento(string SecureCode)
        {
            if (SecureCode.Length > 0 && ValidatonFormat(SecureCode) == true)
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

        public static bool ExistSecureCode(string Renavam)
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
            if (Renavam.Length > 0 && ValidatonFormat(Renavam) == true)
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

        /// <summary>
        /// OBJETIVO: Realizar validação da entrada PLACA com o WebService, chamando o método obterEmissaoCRLV.
        /// </summary>
        /// <param name="placa"></param>
        /// <returns>True ou False, a depender do código de retorno do Webservice</returns>
        public async static Task<bool> ValidationPlaca(string placa)
        {
            if (placa.Length > 0)
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var crlv = await obter.obterEmissaoCRLV(placa);
                if (crlv.codigoRetorno != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// OBJETIVO: Verificar se o veículo possui código de segurança.
        /// </summary>
        /// <returns>True ou False, a depender se o código de segurança em CRLVDialogDetails é maior que 0.</returns>
        public static bool ExistSecureCodePlaca()
        {
            if (Convert.ToDouble(CRLVDialogDetails.codSegurançaOut) > 0)
            {
                CRLVDialogDetails.secureCodeBool = true;
                return true;
            }
            return false;
        }

        public static bool Pendency(string SecureCode)
        {
            //return Api.LerArquivoJson("ano", SecureCode);
            return true;
        }



        public static bool ValidationType(string tipoDeAutorização, string numeroDeAutorizacao, string dataDeAutorizacao)
        {
            return true;
        }

        public static bool Situation(string placa)
        {
            return true;
        }

        public static bool ValidationVehicleRecall()
        {
            return true;
        }

        public static bool ValidationVehicleExemption()
        {
            return true;
        }

        internal static bool ValidatonFormat(string secureCode)
        {
            if (Char.IsDigit(secureCode, 0))
            {
                return true;
            }
            return false;
        }
    }
}
