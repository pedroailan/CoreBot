using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class VehicleLicenseRNTRC
    {
        public static bool ValidationVehicleType(string temRNTRC)
        {
            if(temRNTRC == "S")
            {
               return true;
            }
            else
            {
                return false;
            }
            //return false;
            //if (Api.BuscaJson("rtrc") == true)
            //{
            
            //}
            //return false;
        }

        public static bool ValidationTypeAuthorization(string tipo, string tipoAutorizacaoRNTRCOut)
        {
            if (tipo == "1" && tipoAutorizacaoRNTRCOut == "ETC") return true;
            else if (tipo == "2" && tipoAutorizacaoRNTRCOut == "CTC") return true;
            else if (tipo == "3" && tipoAutorizacaoRNTRCOut == "TAC") return true;
            else return false;
        }

        public static bool ValidationDate(string data)
        {
            if (data.Length >= 8)
            {
                var date = DateTime.Parse(data);

                var dia = date.Day;
                var mes = date.Month;
                var ano = date.Year;

                if (mes < DateTime.Now.Month && ano < DateTime.Now.Year)
                {
                    return false;
                }
                LicenseDialogDetails.dataValidadeRNTRC = ano.ToString() + mes.ToString() + dia.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidationNumber(string nroAutorizacaoRNTRCIn, string nroAutorizacaoRNTRCOut)
        {
            if (Format.Input.ValidationFormat.IsNumber(nroAutorizacaoRNTRCIn) == true)
            {
                if (nroAutorizacaoRNTRCIn == nroAutorizacaoRNTRCOut)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
