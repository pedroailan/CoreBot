// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using CoreBot.Models;
using AdaptiveCards;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PlacaDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public PlacaDialog()
            : base(nameof(PlacaDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeDialog());
            AddDialog(new SpecificationsCRLVeDialog());
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
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe a PLACA do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            LicenseDialogDetails.Placa = stepContext.Result.ToString();

            if (Vehicle.ValidationPlaca(LicenseDialogDetails.Placa) == true)
            {
                if (Vehicle.Situation(LicenseDialogDetails.Placa) == true)
                {
                    if (Vehicle.ExistSecureCodePlaca(LicenseDialogDetails.Placa) == true)
                    {
                        LicenseDialogDetails.SecureCodeBool = true;
                        await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, vou precisar dessa informação");
                        return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), LicenseDialogDetails, cancellationToken);
                    }
                    else
                    {
                        return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), LicenseDialogDetails, cancellationToken);
                    }
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Veículo não autorizado: motivo(s)");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Opa, Esta placa é inválida");
                return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), LicenseDialogDetails, cancellationToken);
            }
        }
    }
}
