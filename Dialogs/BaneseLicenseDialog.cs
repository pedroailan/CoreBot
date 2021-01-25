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
        public CarDialogDetails carDialogDetails;
        public BaneseLicenseDialog()
            : base(nameof(BaneseLicenseDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new CarDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                //ValidatorStepAsync,
                SecureCodeStepAsync,
                OptionStepAsync,
                FinalStepAsync
                
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        

        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        { 
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);
            var promptMessage = MessageFactory.Text("Informe seu RENAVAM" , InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //private async Task<DialogTurnResult> ValidatorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
        //        return await stepContext.EndDialogAsync(cancellationToken);
    
        //}


        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
            string messagetext = null;
            var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        }

        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var renv = stepContext.Result.ToString();

            switch (stepContext.Result.ToString())
            {
                case "1111":
                    return await stepContext.BeginDialogAsync(nameof(CarDialog), cancellationToken);
                case "2222":
                    return await stepContext.BeginDialogAsync(nameof(TruckDialog), cancellationToken);
                default:
                    return await stepContext.BeginDialogAsync(nameof(BaneseLicenseDialog), cancellationToken);
            }
        }



        private static async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var info = "Aqui está o seu Licenciamento!\r\n" +
                       "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                       " - PDF\r\n" +
                       " - Código de Barras";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
