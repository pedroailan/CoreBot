using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    public class Format
    {
        public class Input
        {
            public class ValidationFormat
            {
                public static bool IsNumber(string secureCode)
                {
                    if (Char.IsDigit(secureCode, 0))
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public class Output
        {
            /// <summary>
            /// Método responsável por converter e formatar a data para o tipo dd/MM/aaaa
            /// </summary>
            /// <param name="dataDouble"></param>
            /// <returns></returns>
            public static string FormatData(double dataDouble)
            {
                var cont = 1;
                string dataTotal = "";
                string data = dataDouble.ToString();

                for (int i = 0; i < data.Length; i++)
                {
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont == 4)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont > 4)
                    {
                        dataTotal += data[i].ToString();
                    }
                    cont++;
                }

                return dataTotal;
            }

            public static string FormatData(string data)
            {
                var cont = 1;
                string dataTotal = "";

                for (int i = 0; i < data.Length; i++)
                {
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont == 4)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont > 4)
                    {
                        dataTotal += data[i].ToString();
                    }
                    cont++;
                }

                return dataTotal;
            }

            public static string FormatData(string data, int cont)
            {
                string dataTotal = "";

                for (int i = 0; i < data.Length; i++)
                {
                    //if (cont == 2)
                    //{
                    //    dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    //}
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + data[i + 1].ToString() + data[i + 2].ToString() + "/";
                    }
                    if (cont == 5)
                    {
                        dataTotal += data[i].ToString() + data[i + 1].ToString() + "/";
                    }
                    if (cont > 6)
                    {
                        dataTotal += data[i].ToString() + data[i + 1].ToString();
                        break;
                    }
                    cont++;
                }

                return dataTotal;
            }

            public static string FormatValue(string value)
            {
                string valorTotal = value.TrimStart('0');
                return valorTotal;
            }

            public static string reverseString(string Word)
            {
                char[] arrChar = Word.ToCharArray();
                Array.Reverse(arrChar);
                string invertida = new String(arrChar);

                return invertida;
            }

            public static string reverseDate(string Word)
            {

                var invertida = FormatData(Word, 1);
                var NewDate = DateTime.Parse(invertida, new CultureInfo("pt-PT"));
                invertida = NewDate.Day.ToString() + "0" + NewDate.Month.ToString() +NewDate.Year.ToString();
                return FormatData(invertida);
            }


        }
    }
}
