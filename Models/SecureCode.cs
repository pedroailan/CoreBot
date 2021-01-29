using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class SecureCode
    {
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
        
    }
}
