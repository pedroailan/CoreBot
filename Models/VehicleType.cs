using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class VehicleType
    {
        public static string ValidationVehicleType(string SecureCode)
        {
            if(SecureCode == "Caminhão")
            {
                return SecureCode;
            }
            else
            {
                SecureCode = "Carro";
                return SecureCode;
            }
            
        }
    }
}
