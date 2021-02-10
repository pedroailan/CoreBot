﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootCRLVeDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public RootCRLVeDialog()
            : base(nameof(RootCRLVeDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new PlacaDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new SecureCodeCRLVeDialog());
      
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                SecureCodeQuestionStepAsync,
                SecureCodeStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            await stepContext.Context.SendActivityAsync("Bem-vindo ao serviço de Licenciamento Anual!");
            await stepContext.Context.SendActivityAsync("Aqui você pode emitir o Documento de Circulação de porte obrigatório (CRVL-e).\r\n");
            
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Deseja prosseguir?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {

                return await stepContext.ContinueDialogAsync(cancellationToken);

            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog));
            }
        }


        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Para iniciarmos o processo de emissão do Documento de Circulação de Porte Obrigatório (CRLV-e), vou precisar de algumas informações.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(text), cancellationToken);

            //var choices = new[] { "Baixar PDF" };
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
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
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

                return await stepContext.BeginDialogAsync(nameof(SecureCodeCRLVeDialog), LicenseDialogDetails, cancellationToken);

            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), LicenseDialogDetails, cancellationToken);
            }
        }
    }

}