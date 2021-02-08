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

        public static bool ValidationSecureCode(string SecureCode)
        {
            if (Api.LerArquivoJson("codigodeseguranca", SecureCode) == true)
            {
                return true;
            }
            return false;
        }

        public static bool ExistSecureCode(string Renavam)
        {
            return true;
        }

        public static bool ValidationRenavam(string Renavam)
        {
            if (Renavam == "1234") return true;
            return false;
        }

        public static bool ValidationPlaca(string placa)
        {
            return true;
        }


        public static bool ExistSecureCodePlaca(string placa)
        {
            return true;
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
    }
}
