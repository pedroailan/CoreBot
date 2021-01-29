// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Extensions.Options;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RenavamDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public RenavamDialog()
            : base(nameof(RenavamDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidationRenavamStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            AdaptiveCard card = new AdaptiveCard("1.0")
            {
                Body =
                    {
                        new AdaptiveImage()
                        {
                            Type = "Image",
                            Size = AdaptiveImageSize.Auto,
                            Style = AdaptiveImageStyle.Default,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                            Separator = true,
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg")
                        }
                    }
            };

            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive",
                Name = "cardName"
            }
            ), cancellationToken);

            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o RENAVAM"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            LicenseDialogDetails.Renavam = stepContext.Result.ToString();

            if (Renavam.ValidationRenavam(LicenseDialogDetails.Renavam) ==  true)
            {
                if (Renavam.ExistSecureCode(LicenseDialogDetails.Renavam) == true)
                {
                    await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, vou precisar dessa informação");
                    return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseDialogDetails,cancellationToken);
                }
                else
                {
                    return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseDialogDetails, cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Opa, Renavam Inválido. Acho que você digitou errado!");
                return await stepContext.ReplaceDialogAsync(nameof(RenavamDialog), LicenseDialogDetails, cancellationToken);
            }
        }
    }
}
