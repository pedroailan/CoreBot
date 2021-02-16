using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class VehicleLicenseRNTRC
    {
        public static bool ValidationVehicleType()
        {
            //if(LicenseDialogDetails.temRNTRC == "S")
            //{
            //    return true;
            //}
            //return false;
            //if (Api.BuscaJson("rtrc") == true)
            //{
            return true;
            //}
            //return false;
        }

        public static bool ValidationTypeAuthorization(string tipo)
        {
            switch (tipo)
            {
                case "1":
                    //if(LicenseDialogDetails.tipoAutorizacaoRNTRCOut == "ETC")
                    //{
                    //    return true;
                    //}
                    return true;
                    break;
                case "2":
                    if (LicenseDialogDetails.tipoAutorizacaoRNTRCOut == "CTC")
                    {
                        return true;
                    }
                    break;
                case "3":
                    if (LicenseDialogDetails.tipoAutorizacaoRNTRCOut == "TAC")
                    {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public static bool ValidationDate(string data)
        {
            if (data.Length >= 8)
            {
                var date = DateTime.Parse(data);

                var dia = date.Day;
                var mes = date.Month;
                var ano = date.Year;

                if (mes < DateTime.Now.Month || ano < DateTime.Now.Year)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool ValidationNumber(string nroAutorizacaoRNTRCIn)
        {
            if(nroAutorizacaoRNTRCIn == LicenseDialogDetails.nroAutorizacaoRNTRCOut)
            {
                return true;
            }
            return true;
        }
    }
}
