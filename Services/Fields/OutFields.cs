using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.Fields
{
    public class OutFields
    {
        /// <summary>
        ///Saídas analogas para o metodos
        /// </summary>
        public static int codigoRetorno;
        public static string[] vetDescDebitos = new string[20];
        public static string marcaModelo;
        public static string placa;

        public class Erro
        {
            public static int Codigo;
            public static string Mensagem;
            public static string Trace;
        }
        /// <summary>
        ///Saídas para o método validarServicoLicenciamento
        /// </summary>
        public static int codSegurancaOut;
        public static int renavamOut;
        //public static string placa;
        //public static string marcaModelo;
        public static string nomeProprietario;
        public static string temRNTRC;
        public static string tipoAutorizacaoRNTRC;
        public static string nroAutorizacaoRNTRC;
        public static string temIsençãoIPVA;
        public static string restricao;
        public static string anoLicenciamento;
        public static string contadorAnoLicenciamento;
        public static string recallPendente;
        public static string recallPendenteMensagem;
        public static string recallPendenteDescricao;
        public static double recallPendenteDefeito;
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
        public static int numeroDocumento;
        public static string tipoDocumento;
        public static string cor;
        public static int[] vetTaxas = new int[17];
        //public static string[] vetDescDebitos = new string[20];
        public static int dataProcessamento;
        public static int exercicio;
        public static int ind;
        //public static string marcaModelo;
        public static string nome;
        //public static string placa;
        public static int renavam;
        public static string tipo;
        public static string[] vetValorA = new string[17];
        public static string valorApagar;
        public static int vencimento;
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
        public static int indDescricao;
        public static string[] vetDescInfracao = new string[80];
        public static int indMensagem;
        public static string[] vetDuaMensagem = new string[80];
        public static string chassiSNG;
        public static string tituloVenc;
        public static string datsVenc;
        public static int indParc;
        public static int[] vetDuaParc = new int[9];
        public static string[] vetValorA1Parc = new string[8];
        public static string[] vetLinhaDigParc = new string[51];
        public static string[] vetLinhaCodBarra = new string[44];
        public static string[] vetCodBarraParc = new string[54];
        public static string[] vetASBACE1Parc = new string[25];
        public static string[] vetValorA2Parc = new string[8];
        public static string[] vetValorA3Parc = new string[8];
        public static string[] vetTotalAParc = new string[10];
        public static int[] vetVencimentoParc = new int[8];
        public static string flagParc1A;
        public static string flagParc2A;
        public static string flagParc3A;
    }
}
