﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Generate;
using CoreBot.Models.Generate.Converter;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationsCRLVeDialog : CancelAndHelpDialog
    {

        private CRLVeFields CRLVeFields;


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
                FinalStepAsync,
                //DownloadStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Renavam: " + CRLVeFields.renavam +
                                                                            "\r\nPlaca: " + CRLVeFields.placaOut +
                                                                            "\r\nProprietário: " + CRLVeFields.nomeProprietario),
                                                                            cancellationToken);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Seus dados estão corretos?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Seus dados estão corretos?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Se os dados não estão corretos, teremos que repetir o processo.\r\n" +
                                                            "Caso o problema persista, entre em contato com nossa equipe de atendimento");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), CRLVeFields, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(CRLVeFields.codigoRetorno == 1 && CRLVeFields.erroCodigo == 0) {
                await stepContext.Context.SendActivityAsync("Aqui está o seu Documento de Circulação de Porte Obrigatório (CRLV-e)! Para baixar basta clicar no item abaixo.");
                await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(
                        PdfProvider.Disponibilizer(CRLVeFields.documentoCRLVePdf, "CRLVe_" + CRLVeFields.nomeProprietario)
                    )
                }, cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            } else
            {
                await stepContext.Context.SendActivityAsync("Ocorreu um erro no processamento do PDF, tente novamente mais tarde.");
                return await stepContext.EndDialogAsync(cancellationToken);
            }  
        }
    }
}
