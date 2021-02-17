using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public class Output
        {
            public static string FormatData(double data)
            {
                string dataFormatada = Regex.Replace(data.ToString(),
                @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
               "${day}-${month}-${year}", RegexOptions.None,
               TimeSpan.FromMilliseconds(150));

                //var date = DateTime.Parse(dataFormatada);
                //dataFormatada = date.Day + "/" + date.Month + "/" + date.Year;
                return dataFormatada;
            }
        }
    }
}
