using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Fields
{
    /// <summary>
    /// OBJETIVO: Classe responsável por manter atributos que recebem dados de saída do WebService mediante as entradas de CodSegurança e Placa.
    /// AUTOR(ES): Pedro Ailan e Felipe Falcão
    /// </summary>
    public class CRLVDialogDetails
    {
        public static string codSegurançaOut;
        public static string codSegurancaIn;
        public static string renavam;
        public static string nomeProprietario;
        public static string placaIn;
        public static string placaOut;
        public static int codigoRetorno;
        public static string documentoCRLVePdf;

        public class Erro
        {
            public static int codigo;
            public static string mensagem;
            public static string trace;
        }

        public int Count { get; set; }
        public static bool secureCodeBool { get; set; }
    }
}
