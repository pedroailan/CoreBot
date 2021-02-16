using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class VehicleLicenseExemption
    {
        public static bool Exemption()
        {
            if(LicenseDialogDetails.temIsençãoIPVA == "S")
            {
                return true;
            }
            return false;
        }
    }
}
