using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class VehicleLicenseRecall
    {
        public static bool ValidationVehicleRecall(int recallCodigo)
        {
            if(recallCodigo != 0)
            {
                return true;
            }
            return false;
        }
    }
}
