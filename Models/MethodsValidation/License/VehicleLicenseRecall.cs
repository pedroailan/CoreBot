using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class VehicleLicenseRecall
    {
        public static bool ValidationVehicleRecall()
        {
            if(LicenseDialogDetails.RecallPendente.codigo != 0)
            {
                return true;
            }
            return false;
        }
    }
}
