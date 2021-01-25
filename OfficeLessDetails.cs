// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    public class OfficeLessDetails
    {
        public string Nome { get; set; }
        public string Destination { get; set; }

        public string Origin { get; set; }

        public string TravelDate { get; set; }

        public static explicit operator OfficeLessDetails(string v)
        {
            throw new NotImplementedException();
        }
    }
}
