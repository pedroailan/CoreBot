// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    public class LicenseDialogDetails
    {
        public string Renavam { get; set; }

        public string SecureCode { get; set; }
        public string MarcaModelo { get; set; }
        public string AnoExercicio { get; set; }
        public string Placa { get; set; }
        public string NomeProprietario { get; set; }
        public string Banco { get; set; }
        public string Vehicle { get; set; }
        public string TipoDeAutorização { get; set; }
        public string NumeroDeAutorizacao { get; set; }
        public string DataDeAutorizacao { get; set; }
        public bool SecureCodeBool { get; set; }

        public static explicit operator LicenseDialogDetails(string v)
        {
            throw new NotImplementedException();
        }
    }
}