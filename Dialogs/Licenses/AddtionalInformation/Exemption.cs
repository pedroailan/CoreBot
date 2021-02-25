// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ExemptionDialog : CancelAndHelpDialog
    {
        //private LicenseFields LicenseFields;
        LicenseFields LicenseFields;
        public ExemptionDialog()
            : base(nameof(ExemptionDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RecallDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExemptionStepAsync,
                ExemptionConfirmStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ExemptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Deseja utilizar isenção de IPVA neste veículo?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja utilizar isenção de IPVA neste veículo?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ExemptionConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                LicenseFields.IsencaoIPVA = "S";
                return await stepContext.NextAsync(LicenseFields, cancellationToken);

            }
            else
            {
                LicenseFields.IsencaoIPVA = "N";
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

    }
}
