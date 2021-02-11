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
        public int codigoRetorno;
        public string erro;
        public int erroCodigo;
        public string erroMensagem;
        public string erroTrace;
        public string[] vetDescDebitos = new string[20];
        public string marcaModelo;
        public string placa;

        /// <summary>
        ///Saídas para o método validarServicoLicenciamento
        /// </summary>
        public int codSegurancaOut;
        public int renavamOut;
        //public string placa;
        //public string marcaModelo;
        public string nomeProprietario;
        public string temRNTRC;
        public string tipoAutorizacaoRNTRC;
        public string nroAutorizacaoRNTRC;
        public string temIsençãoIPVA;
        public string restricao;
        public string anoLicenciamento;
        public string contadorAnoLicenciamento;
        public string recallPendente;
        public string recallPendenteMensagem;
        public string recallPendenteDescricao;
        public double recallPendenteDefeito;
        public int contadorRegistro;
        //public string[] vetDescDebitos = new string[20];
        public double[] vetValorCotaUnica = new double[20];
        public double[] vetParcela1 = new double[20];
        public double[] vetParcela2 = new double[20];
        public double[] vetParcela3 = new double[20];
        public double totalParcela1;
        public double totalParcela2;
        public double totalParcela3;
        public double totalCotaUnica;
        public int indiceMensagem;
        public int[] vetCodMensagem = new int[10];
        public string[] vetMensagemDua = new string[80];

        /// <summary>
        ///Saídas para o método efetuarServicoLicenciamento
        /// </summary>
        public string cpfProcurador;
        public int numeroDocumento;
        public string tipoDocumento;
        public string cor;
        public int[] vetTaxas = new int[17];
        //public string[] vetDescDebitos = new string[20];
        public int dataProcessamento;
        public int exercicio;
        public int ind;
        //public string marcaModelo;
        public string nome;
        //public string placa;
        public int renavam;
        public string tipo;
        public string[] vetValorA = new string[17];
        public string valorApagar;
        public int vencimento;
        public string agencia;
        public string mensagem1;
        public string mensagem2;
        public string mensagem3;
        public string mensagem4;
        public string mensagem5;
        public string totalA;
        public string linhaDig;
        public string linhaCodBarra;
        public string codBarra;
        public string asBace1;
        public int indDescricao;
        public string[] vetDescInfracao = new string[80];
        public int indMensagem;
        public string[] vetDuaMensagem = new string[80];
        public string chassiSNG;
        public string tituloVenc;
        public string datsVenc;
        public int indParc;
        public int[] vetDuaParc = new int[9];
        public string[] vetValorA1Parc = new string[8];
        public string[] vetLinhaDigParc = new string[51];
        public string[] vetLinhaCodBarra = new string[44];
        public string[] vetCodBarraParc = new string[54];
        public string[] vetASBACE1Parc = new string[25];
        public string[] vetValorA2Parc = new string[8];
        public string[] vetValorA3Parc = new string[8];
        public string[] vetTotalAParc = new string[10];
        public int[] vetVencimentoParc = new int[8];
        public string flagParc1A;
        public string flagParc2A;
        public string flagParc3A;
    }
}
