// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationsCRLVeDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;


        public SpecificationsCRLVeDialog()
            : base(nameof(SpecificationsCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                ConfirmDataAsync,
                FinalStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Marca/Modelo: " + LicenseDialogDetails.MarcaModelo +
                                                                            "\r\nPlaca: " + LicenseDialogDetails.Placa +
                                                                            "\r\nProprietário: " + LicenseDialogDetails.NomeProprietario),
                                                                            cancellationToken);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Seus dados estão corretos?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Beleza, vamos repetir o processo para outro veículo");
                return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), LicenseDialogDetails, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Define choices
            var choices = new[] { "Baixar Documento de Circulação de Porte Obrigatório (CRLV-e)" };

            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Body =
                {
                    new AdaptiveImage()
                    {
                        Type = "Image",
                        Size = AdaptiveImageSize.Auto,
                        Style = AdaptiveImageStyle.Default,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                        Separator = true,
                        Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
                    }
                },
                Actions = choices.Select(choice => new AdaptiveOpenUrlAction
                {
                    Title = choice,
                    Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

                }).ToList<AdaptiveAction>(),
            };

            // Prompt
            await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(choices),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
                cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
