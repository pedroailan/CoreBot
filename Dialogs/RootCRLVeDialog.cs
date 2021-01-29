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
    public class RootCRLVeDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public RootCRLVeDialog()
            : base(nameof(RootCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeQuestionStepAsync,
                SecureCodeStepAsync,
                SendSecureCodeStepAsync
                //PlateLicenseStepAsync,
                //SecureCodeStepAsync,
                //InfoStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);

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


            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Você pode informar o CÓDIGO DE SEGURANÇA?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
                var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"));
                //var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(RenavamDialog), LicenseDialogDetails, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> SendSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.SecureCode = stepContext.Result.ToString();
            //await stepContext.Context.SendActivityAsync(LicenseDialogDetails.SecureCode);

            if (SecureCode.ValidationSecureCode(LicenseDialogDetails.SecureCode) == 1)
            {
                return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Este código de segurança é inválido, vamos repetir o processo, ok!?");
                return await stepContext.ReplaceDialogAsync(nameof(RootCRLVeDialog), LicenseDialogDetails, cancellationToken);
            }

        }

        //private async Task<DialogTurnResult> PlateLicenseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);
        //    var promptMessage = MessageFactory.Text("Por favor, informe a placa de seu carro", InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        //private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    stepContext.Values["carplate"] = (stepContext.Result.ToString());

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
        //    string messagetext = null;
        //    var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        //}

        //private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    stepContext.Values["securecode"] = (stepContext.Result.ToString());

        //    var info = "Placa: " + stepContext.Values["carplate"] + 
        //               "\r\nCódigo de segurança: " + stepContext.Values["securecode"] + 
        //               "\r\nProprietário: JOSÉ DA SILVA";

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
        //    return await stepContext.ContinueDialogAsync(cancellationToken);
        //}

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{

        //    var info = "Aqui está o seu CRLV-e!\r\n" +
        //               "\r\n";


        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);


        //    // Define choices
        //    var choices = new[] { "Baixar PDF" };

        //    // Create card
        //    var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        //    {
        //        // Use LINQ to turn the choices into submit actions
        //        Actions = choices.Select(choice => new AdaptiveOpenUrlAction
        //        {
        //            Title = choice,
        //            Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

        //        }).ToList<AdaptiveAction>(),
        //    };

        //    // Prompt
        //    return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
        //    {
        //        Prompt = (Activity)MessageFactory.Attachment(new Attachment
        //        {
        //            ContentType = AdaptiveCard.ContentType,
        //            // Convert the AdaptiveCard to a JObject
        //            Content = JObject.FromObject(card),
        //        }),
        //        Choices = ChoiceFactory.ToChoices(choices),
        //        // Don't render the choices outside the card
        //        Style = ListStyle.None,
        //    },
        //        cancellationToken);
        //}

    }
}
