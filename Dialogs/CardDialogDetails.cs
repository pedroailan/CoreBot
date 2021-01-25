// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class CarDialogDetails
    {
        public string Renavam { get; set; }
        public static explicit operator CarDialogDetails(string v)
        {
            throw new NotImplementedException();
        }

    }
}