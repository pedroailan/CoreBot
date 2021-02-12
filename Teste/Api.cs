using CoreBot.Services.ServiceLicenciamento;
using CoreBot.Services.Teste;
using Microsoft.BotBuilderSamples;
using Nancy.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoreBot
{
    public class Api
    {

        LicenseDialogDetails LicenseDialogDetails;
        /// <summary>
        /// API REMOTA
        /// </summary>

        public static async Task ApiRunAsync(string renavam)
        {
            try
            {
                var codRenavam = RestService.For<IApiService>("https://www.detra.gov-se");

                //var info = await codRenavam.GetInfoAsync(renavam);

            }
            catch (Exception e)
            {
                //LicenseDialogDetails.MarcaModelo = e.Message;
            }
        
        }

        


        /// <summary>
        /// Api LOCAL
        /// </summary>
        public static string json = @"{ 
                            ""renavam"" : ""1234"", 
                            ""codigodeseguranca"" : ""1235"", 
                            ""marcamodelo"": ""Fiat Punto"",
                            ""proprietario"": ""Luiz Carlos Brasil"",
                            ""placa"": ""HZD5000"",
                            ""rtrc"": ""false"",
                            ""pendencias"": ""false"",
                            }";

        public static string json2 = @"{ ""renavam"":0123, ""codigodeseguranca"":""0123"", ""rtrc"":""false"", ""marcamodelo"": ""VW GOL"", ""placa"": ""HFJ96D6"", ""proprietario"": ""Ana Carla Júpiter""}";


    public static bool LerArquivoJson(string tipo, string dado)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            dynamic resultado = serializer.DeserializeObject(json2);

            foreach (KeyValuePair<string, object> entry in resultado)
            {
                var key = entry.Key;
                var value = entry.Value as string;
                if (key == tipo)
                {
                    if (value == dado)
                    {
                        Busca("marcamodelo", "propietario");
                        FieldsTest.retornoJson = String.Format("{0} : {1}", key, value);
                        return true;
                    }
                }
            }
            //Console.WriteLine("");
            //LicenseDialogDetails.MarcaModelo = resultado;
            //Console.WriteLine("");
            //Console.ReadKey();
            return false;
        }

        public static void Busca(string marca, string proprietario)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            //var details = new LicenseDialogDetails();
            dynamic resultado = serializer.DeserializeObject(json2);

            foreach (KeyValuePair<string, object> entry in resultado)
            {
                var key = entry.Key;
                var value = entry.Value as string;
                if (key == "marcamodelo") LicenseDialogDetails.marcaModelo = value;
                if (key == "proprietario") LicenseDialogDetails.nomeProprietario = value;
                if (key == "placa") LicenseDialogDetails.placa = value;

            }
        }

        public static bool BuscaJson(string dado)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();


            dynamic resultado = serializer.DeserializeObject(json);



            foreach (KeyValuePair<string, object> entry in resultado)
            {
                var key = entry.Key;
                var value = entry.Value as string;
                if (key == dado && value == "true")
                {
                    return true;
                }
            }
            //Console.WriteLine("");
            //LicenseDialogDetails.MarcaModelo = resultado;
            //Console.WriteLine("");
            //Console.ReadKey();
            return false;
        }
    }
}
