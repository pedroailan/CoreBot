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

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class TruckDialog : CancelAndHelpDialog
    {
        public TruckDialog()
            : base(nameof(TruckDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                YearStepAsync,
                VencimentoStepAsync,
                AuthorizationStepAsync,
                AuthorizationNumberStepAsync,
                AuthorizationDataStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Renavam: 2222\r\nPlaca: ZDC-0202\r\nProprietário/Arrecadário: ANTONIO SILVEIRA SANTOS"), cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> YearStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Deseja pagar o licenciamento de qual ano?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "2021" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> VencimentoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Quer que eu gere o pagamento para qual dia?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Terça-Feira", "Quarta-Feira", "Quinta-Feira", }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AuthorizationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual tipo de autorização você possui?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "ETC - Empresa de transporte de carga", "CTC - Cooperativa de transporte de carga", "TAC - Transportador autônomo de carga", "Não sei onde encontrar esta informação" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AuthorizationNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Por favor, informe a data de validade da autorização", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> AuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Me diga também o número da autorização", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
    }
}
