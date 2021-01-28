using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class SecureCode
    {
        public static bool ValidationSecureCode(string SecureCode)
        {
            if (SecureCode == "1234") return true;
            return false;       
        }
        public static bool ValidationSecureCode(string SecureCode, string Renavam)
        {
            if (SecureCode == "1234") return true;
            return false;
        }
    }
}
