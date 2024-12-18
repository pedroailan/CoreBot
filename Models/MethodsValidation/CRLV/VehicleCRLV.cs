﻿using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.obterEmissaoCRLV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Methods
{
    public class VehicleCRLV
    {
        //public async static Task<bool> ValidationSecureCode(string SecureCode)
        //{
        //    if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true)
        //    {
        //        ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
        //        var crlv = await obter.obterEmissaoCRLV(Convert.ToDouble(SecureCode));
        //        if (CRLVDialogDetails.Erro.codigo == 0)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //    //if (Api.LerArquivoJson("codigodeseguranca", SecureCode) == true)
        //    //{
        //    //    return true;
        //    //}
        //}

        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> ValidationSecureCode(string SecureCode)
        {
            try
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var result = await obter.obterEmissaoCRLV(Convert.ToDouble(SecureCode));
                return result;
            }
            catch (Exception err)
            {
                //erro de conexao
                // atribuir erro
                return null;
            }
        }

        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> ValidationPlaca(string Placa)
        {
            try
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var result = await obter.obterEmissaoCRLV(Placa);
                return result;
            }
            catch (Exception err)
            {
                //erro de conexao
                // atribuir erro
                return null;
            }
        }

        /// <summary>
        /// OBJETIVO: Realizar validação da entrada PLACA com o WebService, chamando o método obterEmissaoCRLV.
        /// </summary>
        /// <param name="placa"></param>
        /// <returns>True ou False, a depender do código de retorno do Webservice</returns>
        //public async static Task<bool> ValidationPlaca(string placa)
        //{
        //    if (placa.Length > 0)
        //    {
        //        ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
        //        var crlv = await obter.obterEmissaoCRLV(placa);
        //        if (crlv.codigoRetorno != 0)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        // <summary>
        /// OBJETIVO: Verificar se o veículo possui código de segurança.
        /// </summary>
        /// <returns>True ou False, a depender se o código de segurança em CRLVDialogDetails é maior que 0.</returns>
        public static bool ExistSecureCodePlaca(string codSegurancaOut)
        {
            if (Convert.ToDouble(codSegurancaOut) > 0)
            {
                //CRLVDialogDetails.secureCodeBool = true;
                return true;
            }
            return false;
        }

        public bool ValidationString(string SecureCode)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true) return true;
            return false;
        }

        public bool ValidationStringPlaca(string placa)
        {
            if (placa != "") return true;
            return false;
        }
    }
}
