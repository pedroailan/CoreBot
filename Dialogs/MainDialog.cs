﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-BR"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new OfficeLessDialog());
            AddDialog(new BaneseLicenseDialog());
            AddDialog(new OtherBanksLicenseDialog());
            AddDialog(new CRLVeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Olá, posso ajudá-lo com alguma das opções abaixo?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Licenciamento Banese", "Licenciamento Outros Bancos", "Emitir CRLV-e", "Nenhuma das alternativas" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            switch (stepContext.Values["choice"].ToString().ToLower())
            {
                case "nenhuma das alternativas":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Que pena! Mas entre em contato com nossa equipe de atendimento que ela vai te ajudar :)"), cancellationToken);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Obrigada!"), cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken);
                case "licenciamento banese":
                    return await stepContext.BeginDialogAsync(nameof(BaneseLicenseDialog), cancellationToken);
                case "licenciamento outros bancos":
                    return await stepContext.BeginDialogAsync(nameof(OtherBanksLicenseDialog), cancellationToken);
                case "emitir crlv-e":
                    return await stepContext.BeginDialogAsync(nameof(CRLVeDialog), cancellationToken);
                default:
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Por favor, insira uma das opções acima")
                    };

                    return await stepContext.BeginDialogAsync(nameof(MainDialog),cancellationToken);
            }
            

            /*var officeLessDetails = new OfficeLessDetails();
            officeLessDetails.Nome = stepContext.Result.ToString();
            if (!string.IsNullOrEmpty(officeLessDetails.Nome)) {
               return await stepContext.BeginDialogAsync(nameof(OfficeLessDialog), officeLessDetails, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                //return await stepContext.NextAsync(-1, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }*/

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //if (stepContext.Result != null)
            //{
            //    var result = stepContext.Result;
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.ToString()), cancellationToken);
            //}
            //else
            //{
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigado."), cancellationToken);
            //}
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
