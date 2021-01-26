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
    public class CarDialog : CancelAndHelpDialog
    {

        private CardDialogDetails cardDialogDetails;

        public CarDialog()
            : base(nameof(CarDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                YearStepAsync,
                VencimentoStepAsync,
                TaxStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
       
        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            cardDialogDetails = (CardDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Renavam: "+ cardDialogDetails.Renavam + " \r\nPlaca: ZDC-0101\r\nProprietário: JOSÉ DA SILVA"), cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> YearStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual ano de exercicio?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "2021" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> VencimentoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual dia você quer pagar?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Terça-Feira", "Quarta-Feira", "Quinta-Feira",}),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> TaxStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            String text = "Foram detectadas alguma(s) multa(s):\r\n " +
                          "________________________________________\r\n" +
                          "Auto: H 591988\r\n " +
                          "Data de Autuação: 08/12/2019\r\n " +
                          "Orgão Autuador: SMTT AJU\r\n" +
                          "Compotência: SMTT AJU\r\n" +
                          "Valor: R$ 195,25\r\n" +
                          "________________________________________" +
                          "\r\nDeseja adiciona-la(s) à via para pagamento?";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(text),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }
    }
}
