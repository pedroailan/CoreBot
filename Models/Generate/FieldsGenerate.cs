using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Generate
{
    public class FieldsGenerate
    {
        //public static string placa = LicenseDialogDetails.Placa;
        public static string placa = "AAA-0000";
        public static string documento = "01099101";
        //public static string nome = LicenseDialogDetails.NomeProprietario;
        public static string nome = "MARCOS PAULO SILVEIRA SANTOS";
        //public static string marca = LicenseDialogDetails.MarcaModelo;
        public static string marca = "NISSAN LIVINA";
        public static string tipo = "AUTOMÓVEL";
        public static string cor = "PRATA";
        public static string exercicio = "2021";
        public static string processado = "05/02/2021";
        public static string emissao = "05/02/2021";
        public static string vencimento = "VENC. LICENCIAMENTO ANO ATUAL:\nCOM DESC IPVA 31/03/2021 SEM DESC IPVA 30/11/2021";
        public static string valorTotal = "R$ 1.872,00";
        public static string validadeDUA = "01/01/2022";
        public static string numCodBarras = "856500000059 888400260020 281498960208 210331000005";
        
        public static List<string> listaTaxas()
        {
            List<string> taxa = new List<string>();
            taxa.Add("IPVA 2021 BASE");
            taxa.Add("MULTA 12/01/2020");

            return taxa;
        }

        public static List<string> listaPreco()
        {
            List<string> preco = new List<string>();
            preco.Add("R$ 502,00");
            preco.Add("R$ 294,30");

            return preco;
        }

        public static List<string> obs()
        {
            List<string> obs = new List<string>();
            return obs;
        }

        public static List<string> pendencias()
        {
            List<string> pendencias = new List<string>();
            pendencias.Add("PENDENCIA");
            return pendencias;
        }

    }
}
