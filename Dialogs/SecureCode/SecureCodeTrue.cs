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
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SecureCodeDialog : CancelAndHelpDialog
    {

        private CarDialogDetails carDialogDetails;

        public SecureCodeDialog()
            : base(nameof(SecureCodeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                ConfirmDataAsync,
                YearStepAsync,
                FinalStepAsync
                //TaxStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            carDialogDetails = (CarDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Marca/Modelo: " + carDialogDetails.MarcaModelo + 
                                                                            "\r\nPlaca: " + carDialogDetails.Placa + 
                                                                            "\r\nProprietário: " + carDialogDetails.NomeProprietario), 
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
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);

            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(RootDialog), cancellationToken);
            }
        }




        private async Task<DialogTurnResult> YearStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(SecureCode.Pendency(carDialogDetails.SecureCode) == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Qual ano de exercicio?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "2020" , "2021" }),
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }
            
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var info = "Aqui está sua via para pagamento no BANESE!\r\n" +
                        "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n";

            var code = "Código de Barras: 00001222 222525 56599595 5544444";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(code), cancellationToken);


            // Define choices
            var choices = new[] { "Baixar PDF" };

            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = choices.Select(choice => new AdaptiveOpenUrlAction
                {
                    Title = choice,
                    Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

                }).ToList<AdaptiveAction>(),
            };

            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
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
        }

    }
}
