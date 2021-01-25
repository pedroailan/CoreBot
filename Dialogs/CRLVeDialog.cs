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
    public class CRLVeDialog : CancelAndHelpDialog
    {

        public CRLVeDialog()
            : base(nameof(CRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PlateLicenseStepAsync,
                SecureCodeStepAsync,
                InfoStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> PlateLicenseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);
            var promptMessage = MessageFactory.Text("Por favor, informe a placa de seu carro", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["carplate"] = (stepContext.Result.ToString());

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
            string messagetext = null;
            var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        }

        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["securecode"] = (stepContext.Result.ToString());

            var info = "Placa: " + stepContext.Values["carplate"] + 
                       "\r\nCódigo de segurança: " + stepContext.Values["securecode"] + 
                       "\r\nProprietário: JOSÉ DA SILVA";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private static async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var info = "Aqui está o seu CRLV-e!\r\n" +
                       "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                       " - PDF\r\n" +
                       " - Código de Barras";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
