// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    public class CardDialogDetails
    {
        public string Renavam { get; set; }

        public string SecureCode { get; set; }

        public static explicit operator CardDialogDetails(string v)
        {
            throw new NotImplementedException();
        }
    }
}