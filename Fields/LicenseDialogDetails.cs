// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// OBJETIVO: Classe responsável por manter atributos que recebem dados de saída do WebService mediante as entradas de CodSegurança e Placa.
    /// IN: entrada de dados pelo usuário / OUT: entrada de dados pelo WebService
    /// AUTOR(ES): Pedro Ailan e Felipe Falcão
    /// </summary>
    public class LicenseDialogDetails
    {
        public static int codigoRetorno;
        public static string[] vetDescDebitos = new string[25];
        public static string marcaModelo;
        public static string placa;
        public static string renavamOut;
        public static bool secureCodeBool;

        public class Erro
        {
            public static int codigo;
            public static string mensagem;
            public static string trace;
        }

        public static string codSegurancaIn;
        public static string renavamIn;

        public class RecallPendente
        {
            public static int codigo;
            public static string mensagem;
            
            public class listaRecall
            {
                public static string[] descricao;
                public static double defeito;
            }
        }
        /// <summary>
        ///Saídas para o método validarServicoLicenciamento
        /// </summary>
        public static string codSegurancaOut;
        //public static string renavamOut;
        //public static string placa;
        //public static string marcaModelo;
        public static string nomeProprietario;
        public static string temRNTRC;
        public static string tipoAutorizacaoRNTRCIn;
        public static string tipoAutorizacaoRNTRCOut;
        public static string nroAutorizacaoRNTRCIn;
        public static string nroAutorizacaoRNTRCOut;
        public static string dataValidadeRNTRC;
        public static string temIsençãoIPVA;
        public static string restricao;
        public static double[] anoLicenciamento = new double[4];
        public static double contadorAnoLicenciamento;
        public static int contadorRegistro;
        //public static string[] vetDescDebitos = new string[20];
        public static double[] vetValorCotaUnica = new double[20];
        public static double[] vetParcela1 = new double[20];
        public static double[] vetParcela2 = new double[20];
        public static double[] vetParcela3 = new double[20];
        public static double totalParcela1;
        public static double totalParcela2;
        public static double totalParcela3;
        public static double totalCotaUnica;
        public static int indiceMensagem;
        public static int[] vetCodMensagem = new int[10];
        public static string[] vetMensagemDua = new string[80];

        /// <summary>
        ///Saídas para o método efetuarServicoLicenciamento
        /// </summary>
        public static string cpfProcurador;
        public static double numeroDocumento;
        public static string tipoDocumentoIn;
        public static string tipoDocumentoOut;
        public static string cor;
        public static double[] vetTaxas = new double[17];
        //public static string[] vetDescDebitos = new string[20];
        public static double dataProcessamento;
        public static double exercicio;
        public static double ind;
        //public static string marcaModelo;
        public static string nome;
        //public static string placa;
        //public static string renavamOut;
        public static string tipo;
        public static string[] vetValorA = new string[17];
        public static string valorApagar;
        public static double vencimento;
        public static string agencia;
        public static string mensagem1;
        public static string mensagem2;
        public static string mensagem3;
        public static string mensagem4;
        public static string mensagem5;
        public static string totalA;
        public static string linhaDig;
        public static string linhaCodBarra;
        public static string codBarra;
        public static string asBace1;
        public static double indDescricao;
        public static string[] vetDescInfracao = new string[80];
        public static double indMensagem;
        public static string[] vetDuaMensagem = new string[80];
        public static string chassiSNG;
        public static string tituloVenc;
        public static string datsVenc;
        public static double indParc;
        public static double[] vetDuaParc = new double[9];
        public static string[] vetValorA1Parc = new string[8];
        public static string[] vetLinhaDigParc = new string[51];
        public static string[] vetLinhaCodBarra = new string[44];
        public static string[] vetCodBarraParc = new string[54];
        public static string[] vetASBACE1Parc = new string[25];
        public static string[] vetValorA2Parc = new string[8];
        public static string[] vetValorA3Parc = new string[8];
        public static string[] vetTotalAParc = new string[10];
        public static double[] vetVencimentoParc = new double[8];
        public static string flagParc1A;
        public static string flagParc2A;
        public static string flagParc3A;

        public static string Banco { get; internal set; }
        public static int Count { get; internal set; }
        public static bool SecureCodeBool { get; internal set; }

        public static string IsencaoIPVA;
        public static double[] anoLicenciamentoIn;
    }
}