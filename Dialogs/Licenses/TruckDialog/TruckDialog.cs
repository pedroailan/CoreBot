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
        private LicenseDialogDetails LicenseDialogDetails;
        public TruckDialog()
            : base(nameof(TruckDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AuthorizationStepAsync,
                AuthorizationNumberStepAsync,
                AuthorizationDataStepAsync,
                AuthorizationValidationStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
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
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            LicenseDialogDetails.TipoDeAutorização = stepContext.Values["choice"].ToString().ToLower();
            var promptMessage = MessageFactory.Text("Para continuarmos informe o número da autorização", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> AuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.NumeroDeAutorizacao = stepContext.Result.ToString();
            
            var promptMessage = MessageFactory.Text("Por fim, informe a data de validade da autorização\r\n" +
                                                     "Exemplo: 12/12/2021", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            
        }


        private async Task<DialogTurnResult> AuthorizationValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.DataDeAutorizacao = stepContext.Result.ToString();
            ///Valida tipo da autorização
            if (Authotization.ValidationType(LicenseDialogDetails.TipoDeAutorização, LicenseDialogDetails.NumeroDeAutorizacao, LicenseDialogDetails.DataDeAutorizacao) == true)
            {

                return await stepContext.EndDialogAsync(LicenseDialogDetails, cancellationToken);

            }
            else
            {
                await stepContext.Context.SendActivityAsync("Os dados informados não estão de acordo com o sistema!\r\n" +
                                                           "Vou ter que repetir algumas perguntas, ok?");
                return await stepContext.ReplaceDialogAsync(nameof(TruckDialog), LicenseDialogDetails, cancellationToken);
            }
        }

    }
}
