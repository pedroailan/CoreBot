// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// OBJETIVO: Classe responsável por manter atributos que recebem dados de saída do WebService mediante as entradas de CodSegurança e Placa.
    /// AUTOR(ES): Pedro Ailan
    /// </summary>
    public class LicenseDialogDetails
    {
        /// ENTRADA
        public static string Renavam { get; set; }
        public static string SecureCode { get; set; }
        public static string TipoDeAutorização { get; set; }
        public static string AnoExercicio { get; set; }
        public static string Placa { get; set; }
        public static string NumeroDeAutorizacao { get; set; }
        public static string DataDeAutorizacao { get; set; }
        public string Banco { get; set; }
        public static string IsencaoIPVA { get; set; }

        /// SAIDA
        public static string NomeProprietario { get; set; }
        public static string MarcaModelo { get; set; }
        public static string Vehicle { get; set; }
        public bool SecureCodeBool { get; set; }
        public int Count { get; set; }
        public string TipoDocumento { get; set; }

        /// EXCEÇÂO
        public static string Error { get; set; }
    }
}