using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class Renavam
    {
        public static bool ExistSecureCode (string Renavam)
        {
            return true;
        }

        public static bool ValidationRenavam(string Renavam)
        {
            if (Renavam == "1234") return true;
            return false;
        }
    }
}
