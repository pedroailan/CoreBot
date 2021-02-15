using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class Format
    {
        public class Input
        {
            public static bool ValidationFormat(string secureCode)
            {
                if (Char.IsDigit(secureCode, 0))
                {
                    return true;
                }
                return false;
            }
        }

    }
}
