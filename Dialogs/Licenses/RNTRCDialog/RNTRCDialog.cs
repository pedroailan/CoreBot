// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Models;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RNTRCDialog : CancelAndHelpDialog
    {
        private LicenseDialogDetails LicenseDialogDetails;
        public RNTRCDialog()
            : base(nameof(RNTRCDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AuthorizationStepAsync,
                AuthorizationNumberStepAsync,
                ValidationTypeAuthorizationStepAsync,
                ValidationAuthorizationNumeroStepAsync,
                AuthorizationDataStepAsync,
                ValidationAuthorizationDataStepAsync,
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
            LicenseDialogDetails.tipoAutorizacaoRNTRCIn = stepContext.Context.Activity.Text;
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationTypeAuthorizationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
        //    if(VehicleLicenseRNTRC.ValidationTypeAuthorization(LicenseDialogDetails.tipoAutorizacaoRNTRCIn) == true)
        //    {
                var promptMessage = MessageFactory.Text("Para continuarmos informe o número da autorização", InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }
        //    else
        //    {
        //        await stepContext.Context.SendActivityAsync("Não é esse o tipo de autorização que consta em nossos sistemas, vamos repetir!");
        //        return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseDialogDetails, cancellationToken);
        //    }
        }


        private async Task<DialogTurnResult> ValidationAuthorizationNumeroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.nroAutorizacaoRNTRCIn = stepContext.Result.ToString();
            //if(VehicleLicenseRNTRC.ValidationNumber(LicenseDialogDetails.nroAutorizacaoRNTRCIn) == true)
            //{
                return await stepContext.ContinueDialogAsync(cancellationToken);
            //}
            //else
            //{
            //    await stepContext.Context.SendActivityAsync("Não é esse o numero de autorização que consta em nossos sistemas, vamos repetir!");
            //    return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseDialogDetails, cancellationToken);
            //}
            
        }

        private async Task<DialogTurnResult> AuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Por fim, informe a data de validade da autorização\r\n" +
                                                     "Exemplo: 12/12/2021", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> ValidationAuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.dataValidadeRNTRC = stepContext.Result.ToString();

            if (VehicleLicenseRNTRC.ValidationDate(LicenseDialogDetails.dataValidadeRNTRC) == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("A data informada deve ser maior que a atual, vamos repetir o processo");
                return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseDialogDetails, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> AuthorizationValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            ///Valida tipo da autorização
            if (VehicleLicenseRNTRC.ValidationNumber(LicenseDialogDetails.nroAutorizacaoRNTRCIn) == true && VehicleLicenseRNTRC.ValidationTypeAuthorization(LicenseDialogDetails.tipoAutorizacaoRNTRCIn))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Os dados informados não estão de acordo com o nosso sistema.\r\n" +
                                                           "Vou ter que repetir algumas perguntas, ok?");
                return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseDialogDetails, cancellationToken);
            }
        }

    }
}
