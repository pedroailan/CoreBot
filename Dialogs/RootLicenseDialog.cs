﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootLicenseDialog : CancelAndHelpDialog
    {

        //private LicenseDialogDetails LicenseDialogDetails;
        LicenseFields LicenseFields;
        public RootLicenseDialog()
            : base(nameof(RootLicenseDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-BR"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new RenavamDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new SecureCodeDialog());

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
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;
            await stepContext.Context.SendActivityAsync("Bem-vindo ao serviço de Licenciamento Anual! \r\n" + Emojis.Veiculos.Carro + stepContext.Context.Activity.ChannelData);
            await stepContext.Context.SendActivityAsync("Aqui você pode gerar o documento para pagar o licenciamento do seu veículo.\r\n" +
                                                        "O documento gerado aqui é o Documento de Arrecadação (DUA).");
            
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + Emojis.Rostos.SorrisoSuor + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
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
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);

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
                Prompt = MessageFactory.Text($"Você pode informar o CÓDIGO DE SEGURANÇA?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você pode informar o CÓDIGO DE SEGURANÇA?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                
                return await stepContext.BeginDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
                
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(RenavamDialog), LicenseFields, cancellationToken);
            }
        }
    }
}
       