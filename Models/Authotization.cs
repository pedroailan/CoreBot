// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    internal class Authotization
    {
        internal static bool ValidationType(string tipoDeAutorização, string numeroDeAutorizacao, string dataDeAutorizacao)
        {
            return true;
        }

        internal static bool ValidationNumber(string numeroDeAutorizacao)
        {
            return true;
        }

        internal static bool ValidationDate(string dataDeAutorizacao)
        {
            return true;
        }
    }
}