using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Generate
{
    /// <summary>
    /// OBJETIVO: Fields de preenchimento das variáveis necessárias para construção dos PDFs.
    /// AUTOR(ES): Felipe Falcão
    /// </summary>
    public class FieldsGenerate
    {
        ///// <summary>
        ///// Fields utilizados na Ficha de Compensação e DUA
        ///// </summary>
        //public static string placa = LicenseDialogDetails.placa;
        //public static string documento = LicenseDialogDetails.numeroDocumento.ToString();
        //public static string nome = LicenseDialogDetails.nomeProprietario;
        //public static string marca = LicenseDialogDetails.marcaModelo;
        //public static string tipo = LicenseDialogDetails.tipo;
        //public static string cor = LicenseDialogDetails.cor;
        //public static string exercicio = LicenseDialogDetails.exercicio.ToString();
        //public static string processado = LicenseDialogDetails.dataProcessamento.ToString();
        //public static string emissao = LicenseDialogDetails.dataProcessamento.ToString();
        //public static string dataVenc = LicenseDialogDetails.datsVenc; // Texto explicativo
        //public static string valorTotal = LicenseDialogDetails.valorApagar;
        //public static string vencimento = LicenseDialogDetails.vencimento.ToString();
        //public static string numCodBarras = LicenseDialogDetails.codBarra;
        //public static string lerCodBarras = LicenseDialogDetails.linhaCodBarra;
        //public static string validade = LicenseDialogDetails.vencimento.ToString();
        //public static string chassi = LicenseDialogDetails.chassiSNG;

        ///// <summary>
        ///// Fields utilizados somente na Ficha de Compensação
        ///// </summary>
        //public static string nossonum = "285600816";
        //public static string endereco = "RUA DOIS, TANCREDO NEVES";
        //public static string cep = "49000-000";
        //public static string cpf = "000.000.000-00";
        //public static string sacador = "AVENIDA TANCREDO NEVES, S/N, ARACAJU/SE";

        ///// <summary>
        ///// Ocorrência de listas
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> listaTaxas()
        //{
        //    List<string> taxa = new List<string>();

        //    foreach (string value in LicenseDialogDetails.vetDescDebitos)
        //    {
        //        taxa.Add(value);
        //    }

        //    return taxa;
        //}

        //public static List<string> listaPreco()
        //{
        //    List<string> preco = new List<string>();
        //    foreach (int value in LicenseDialogDetails.vetTaxas)
        //    {
        //        preco.Add("R$ " + value.ToString());
        //    }
        //    return preco;
        //}

        //public static List<string> multas()
        //{
        //    List<string> multas = new List<string>();
        //    foreach (string value in LicenseDialogDetails.vetDescInfracao)
        //    {
        //        multas.Add(value);
        //    }
        //    return multas;
        //}

        //public static List<string> obs()
        //{
        //    List<string> obs = new List<string>();
        //    obs.Add("- ESTE DUA TAMBÉM PODE SER PAGO POR CARTÕES DE CRÉDITO EM SERVIÇO DISPONIBILIZADO NO SITE DO DETRAN(WWW.DETRAN.SE.GOV.BR)");
        //    return obs;
        //}

        //public static List<string> pendencias()
        //{
        //    List<string> pendencias = new List<string>();
        //    return pendencias;
        //}

        public static string placa = "AAA-0000";
        public static string documento = "01099101";
        public static string nome = "MARCOS PAULO SILVEIRA SANTOS";
        public static string marca = "NISSAN LIVINA";
        public static string tipo = "AUTOMÓVEL";
        public static string cor = "PRATA";
        public static string exercicio = "2021";
        public static string processado = "05/02/2021";
        public static string emissao = "05/02/2021";
        public static string dataVenc = "VENC. LICENCIAMENTO ANO ATUAL:\nCOM DESC IPVA 31/03/2021 SEM DESC IPVA 30/11/2021";
        public static string valorTotal = "R$ 1.872,00";
        public static string validadeDUA = "01/01/2022";
        public static string numCodBarras = "856500000059888400260020281498960208210331000005";
        public static string lerCodBarras = "856500000059 888400260020 281498960208 210331000005";
        public static string vencimento = "a";
        public static string chassi = "a";



        public static string nossonum = "285600816";
        public static string endereco = "RUA DOIS, TANCREDO NEVES";
        public static string cep = "49000-000";
        public static string cpf = "000.000.000-00";
        public static string sacador = "AVENIDA TANCREDO NEVES, S/N, ARACAJU/SE";
        

        public static List<string> listaTaxas()
        {
            List<string> taxa = new List<string>();
            taxa.Add("IPVA 2021 VALOR BASE");
            taxa.Add("IPVA 2021 VL DESCONTO");
            taxa.Add("LIC. ANO ATUAL");
            taxa.Add("MULTAS SMTT DES CONV.2016");

            return taxa;
        }

        public static List<string> listaPreco()
        {
            List<string> preco = new List<string>();
            preco.Add("R$ 502,00");
            preco.Add("R$ 47,30");
            preco.Add("R$ 136,50");
            preco.Add("R$ 294,30");

            return preco;
        }

        public static List<string> multas()
        {
            List<string> multas = new List<string>();
            multas.Add("AUTO : M 37214231 DEIXAR D/DAR PREFERENCIA D/PASSAGEM A PEDESTRE/VEIC.NAO MOT.\n" +
                        "LOCAL:AVENIDA SANTOS DUMONT EM FRENT 07:54 20/01/2021");

            return multas;
        }

        public static List<string> obs()
        {
            List<string> obs = new List<string>();
            obs.Add("- ESTE DUA TAMBÉM PODE SER PAGO POR CARTÕES DE CRÉDITO EM SERVIÇO DISPONIBILIZADO NO SITE DO DETRAN(WWW.DETRAN.SE.GOV.BR)");
            return obs;
        }

        public static List<string> pendencias()
        {
            List<string> pendencias = new List<string>();
            return pendencias;
        }

    }
}
