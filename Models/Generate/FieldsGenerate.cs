using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.BotBuilderSamples;
using System.Collections.Generic;

namespace CoreBot.Models.Generate
{
    /// <summary>
    /// OBJETIVO: Fields de preenchimento das variáveis necessárias para construção dos PDFs.
    /// AUTOR(ES): Felipe Falcão
    /// </summary>
    /// 
    ///blabla
    public class FieldsGenerate
    {
        public string Fields(LicenseFields dados)
        {
            return dados.nomeProprietario;
        }
        /// <summary>
        /// Fields utilizados na Ficha de Compensação e DUA
        /// </summary>
        LicenseDialogDetails dados = new LicenseDialogDetails();
        public string placa = LicenseDialogDetails.placa;
        public string documento = LicenseDialogDetails.numeroDocumento.ToString();
        //public string nome = LicenseDialogDetails.nomeProprietario;
        public string nome()
        {
            return dados.nomeProprietario;
        }
        public string marca = LicenseDialogDetails.marcaModelo;
        public string tipo = LicenseDialogDetails.tipo;
        public string cor = LicenseDialogDetails.cor;
        public string exercicio = LicenseDialogDetails.exercicio.ToString();
        public string processado = Format.Output.FormatData(LicenseDialogDetails.dataProcessamento);
        public string emissao = Format.Output.FormatData(LicenseDialogDetails.dataProcessamento);
        public string dataVenc = LicenseDialogDetails.datsVenc; // Texto explicativo
        public string tituloVenc = LicenseDialogDetails.tituloVenc;
        public string valorTotal = Format.Output.FormatValue(LicenseDialogDetails.valorApagar);
        public string vencimento = Format.Output.reverseDate(LicenseDialogDetails.vencimento.ToString());
        public string validade = Format.Output.FormatData(LicenseDialogDetails.vencimento);
        public string chassi = LicenseDialogDetails.chassiSNG;
        public string mensagem1 = LicenseDialogDetails.mensagem1;
        public string mensagem2 = LicenseDialogDetails.mensagem2;
        public string mensagem3 = LicenseDialogDetails.mensagem3;
        public string mensagem4 = LicenseDialogDetails.mensagem4;
        public string mensagem5 = LicenseDialogDetails.mensagem5;

        /// <summary>
        /// Fields utilizados somente no DUA
        /// </summary>
        public string linhaDig = LicenseDialogDetails.linhaDig;

        /// <summary>
        /// Fields utilizados somente na Ficha de Compensação
        /// </summary>
        public string nossonum = LicenseDialogDetails.nossoNumero;
        public string endereco = "";
        public string cep = LicenseDialogDetails.cepPagador;
        public string cpf = LicenseDialogDetails.cpfCnpjPagador;
        public string sacador = LicenseDialogDetails.enderecoPagador + ", " + LicenseDialogDetails.bairroPagador + " - "
                                        + LicenseDialogDetails.municipioPagador + "/" + LicenseDialogDetails.ufPagador
                                        + ". CEP: " + LicenseDialogDetails.cepPagador;
        public string agencia = LicenseDialogDetails.agencia;

        public string totalA = LicenseDialogDetails.totalA;
        public string linhaCodBarras = LicenseDialogDetails.linhaCodBarra;
        public string codBarras = LicenseDialogDetails.codBarra;
        public string asBace1 = LicenseDialogDetails.asBace1;

        /// <summary>
        /// Ocorrência de listas
        /// </summary>
        /// <returns></returns>
        public List<string> listaTaxas()
        {
            List<string> taxa = new List<string>();

            foreach (string value in LicenseDialogDetails.vetDescDebitos)
            {
                if (value != "")
                {
                    taxa.Add(value);
                }
            }

            return taxa;
        }

        public List<string> listaPreco()
        {
            List<string> preco = new List<string>();
            foreach (string value in LicenseDialogDetails.vetValorA)
            {
                if (value != "")
                {
                    preco.Add("R$ " + Format.Output.FormatValue(value));
                }
                else
                {
                    break;
                }
            }
            return preco;
        }

        public List<string> multas()
        {
            List<string> multas = new List<string>();
            foreach (string value in LicenseDialogDetails.vetDescInfracao)
            {
                multas.Add(value);
            }
            return multas;
        }

        public List<string> obs()
        {
            List<string> obs = new List<string>();
            obs.Add("- " + LicenseDialogDetails.mensagem1 + " " + LicenseDialogDetails.mensagem2);
            return obs;
        }

        public List<string> pendencias()
        {
            List<string> pendencias = new List<string>();
            return pendencias;
        }

        //public static string placa = "AAA-0000";
        //public static string documento = "01099101";
        //public static string nome = "MARCOS PAULO SILVEIRA SANTOS";
        //public static string marca = "NISSAN LIVINA";
        //public static string tipo = "AUTOMÓVEL";
        //public static string cor = "PRATA";
        //public static string exercicio = "2021";
        //public static string processado = "05/02/2021";
        //public static string emissao = "05/02/2021";
        //public static string dataVenc = "VENC. LICENCIAMENTO ANO ATUAL:\nCOM DESC IPVA 31/03/2021 SEM DESC IPVA 30/11/2021";
        //public static string valorTotal = "R$ 1.872,00";
        //public static string validadeDUA = "01/01/2022";
        //public static string numCodBarras = "856500000059888400260020281498960208210331000005";
        //public static string lerCodBarras = "856500000059 888400260020 281498960208 210331000005";
        //public static string vencimento = "a";
        //public static string chassi = "a";



        //public static string nossonum = "285600816";
        //public static string endereco = "RUA DOIS, TANCREDO NEVES";
        //public static string cep = "49000-000";
        //public static string cpf = "000.000.000-00";
        //public static string sacador = "AVENIDA TANCREDO NEVES, S/N, ARACAJU/SE";


        //public static List<string> listaTaxas()
        //{
        //    List<string> taxa = new List<string>();
        //    taxa.Add("IPVA 2021 VALOR BASE");
        //    taxa.Add("IPVA 2021 VL DESCONTO");
        //    taxa.Add("LIC. ANO ATUAL");
        //    taxa.Add("MULTAS SMTT DES CONV.2016");

        //    return taxa;
        //}

        //public static List<string> listaPreco()
        //{
        //    List<string> preco = new List<string>();
        //    preco.Add("R$ 502,00");
        //    preco.Add("R$ 47,30");
        //    preco.Add("R$ 136,50");
        //    preco.Add("R$ 294,30");

        //    return preco;
        //}

        //public static List<string> multas()
        //{
        //    List<string> multas = new List<string>();
        //    multas.Add("AUTO : M 37214231 DEIXAR D/DAR PREFERENCIA D/PASSAGEM A PEDESTRE/VEIC.NAO MOT.\n" +
        //                "LOCAL:AVENIDA SANTOS DUMONT EM FRENT 07:54 20/01/2021");

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

    }
}
