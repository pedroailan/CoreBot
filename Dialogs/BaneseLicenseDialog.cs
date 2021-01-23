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
    public class BaneseLicenseDialog : CancelAndHelpDialog
    {

        public BaneseLicenseDialog()
            : base(nameof(BaneseLicenseDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidatorStepAsync,
                SecureCodeStepAsync,
                FinalStepAsync
                
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

   

        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Você optou pela opção BANESE"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos a negociação, vou precisar de algumas informações."), cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Você pode fornecer seu Renavam?") }, cancellationToken);

        }

        private async Task<DialogTurnResult> ValidatorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result == true)
            {

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe seu Renavam"), cancellationToken);
                string messagetext = null;
                var promptMessage = MessageFactory.Text( messagetext , InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }


        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
            string messagetext = null;
            var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        }



        private static async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Aqui está:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("- PDF"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("- Codigo de Barras"), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
