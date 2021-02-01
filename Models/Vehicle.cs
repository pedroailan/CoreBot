using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class Vehicle
    {
        public static string ValidationVehicleType(string SecureCode)
        {
            if (SecureCode == "Caminhão")
            {
                return SecureCode;
            }
            else
            {
                SecureCode = "Carro";
                return SecureCode;
            }

        }

        public static int ValidationSecureCode(string SecureCode)
        {
            if (SecureCode == "1234") return 1;
            else if (SecureCode == "1235")
            {
                return 2;
            }
            else
            {
                return 0;
            }
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
    }
}
