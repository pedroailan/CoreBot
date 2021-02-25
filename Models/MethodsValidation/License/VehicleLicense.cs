using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.validarServicoLicenciamento;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    /// <summary>
    /// Esta classe contém o metodos básicos de validação para lincenciamento.
    /// </summary>
    public class VehicleLicense
    {
        /// <summary>
        /// OBJETIVO: Validar formato de entrada e realizar chamada ao metodo assincrono passando como parâmetro
        ///           somento o código de segurança.
        /// AUTOR(ES): Pedro Ailan
        /// </summary>
        /// <param name="SecureCode"></param>
        /// <returns></returns>
        /// 
        public bool ValidationString(string SecureCode)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true) return true;
            return false;
        }

        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> ValidationSecureCodeLicenciamento(string SecureCode, string tipoDocumentoIn)
        {
            try
            {
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var result = await obter.validarServicoLicenciamento(0, Convert.ToDouble(SecureCode), tipoDocumentoIn, 0);
                return result;
            }
            catch (Exception err)
            {
                //erro de conexao
                // atribuir erro
                return null;
            }

            return null;
        }

        public async static Task<bool> ValidationSecureCodeLicenciamento(string SecureCode, double year)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true)
            {
                //16736005660
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                await obter.validarServicoLicenciamento(0, Convert.ToDouble(SecureCode), LicenseDialogDetails.tipoDocumentoIn, year);
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

        /// <summary>
        /// OBJETIVO: Verificar a existência de código de segurança, quando a entrada foi realizada somento com o renavam.
        /// AUTOR(ES): Pedro Ailan
        /// </summary>
        /// <returns></returns>
        public static bool ExistSecureCode(string codSegurancaOut)
        {
            if (Convert.ToDouble(LicenseDialogDetails.codSegurancaOut) > 0)
            {
                //LicenseDialogDetails.secureCodeBool = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// OBJETIVO: Validar formato de entrada e realizar chamada ao metodo assincrono passando como parâmetro
        ///           somento renavam.
        /// AUTOR(ES): Pedro Ailan
        /// </summary>
        /// <param name="Renavam"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> ValidationRenavam(string Renavam, string tipoDocumentoIn)
        {

            try
            {
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var result = await obter.validarServicoLicenciamento(Convert.ToDouble(Renavam), 0, tipoDocumentoIn, 0);
                return result;
            }
            catch (Exception err)
            {
                //erro de conexao
                // atribuir erro
                return null;
            }

            return null;
            //if (Renavam.Length > 0 && Format.Input.ValidationFormat.IsNumber(Renavam) == true)
            //{
            //    //16736005660
            //    ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
            //    await obter.validarServicoLicenciamento(Convert.ToDouble(Renavam), 0, tipoDocumentoIn, 0);
            //    if (LicenseDialogDetails.Erro.codigo == 0)
            //    {
            //        return true;
            //    }
            //}
            //return false;
            //if (Renavam == "1234") return true;
            //return false;
        }

        /// <summary>
        /// OBJETIVO: Verificar a existência de pendencias, mediante o ano de licenciamento.
        /// AUTOR(ES): Pedro Ailan
        /// </summary>
        /// <returns></returns>
        public static bool Pendency(double[] anoLicenciamento)
        {
            if (anoLicenciamento != null)
            {
                return true;
            }
            return false;
        }

        public static bool Situation(string placa)
        {
            return true;
        }

        /// <summary>
        /// OBJETIVO: Verificar quantidade de anos que existem para lincenciar.
        /// AUTOR(ES): Pedro Ailan
        /// </summary>
        /// <returns></returns>
        public static bool ValidationYear()
        {
            if (LicenseDialogDetails.contadorAnoLicenciamento > 1)
            {
                return true;
            }
            return false;
        }
    }


}
