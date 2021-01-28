// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    public class LicenseDialogDetails
    {
        public string Renavam { get; set; }

        public string SecureCode { get; set; }
        public string MarcaModelo { get; internal set; }
        public string AnoExercicio { get; internal set; }
        public string Placa { get; internal set; }
        public string NomeProprietario { get; internal set; }

        public static explicit operator LicenseDialogDetails(string v)
        {
            throw new NotImplementedException();
        }
    }
}