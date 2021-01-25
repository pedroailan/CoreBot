// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class OtherBanksLicenseDialog : CancelAndHelpDialog
    {

        private CardDialogDetails cardDialogDetails;

        public OtherBanksLicenseDialog()
            : base(nameof(OtherBanksLicenseDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new CarDialog());
            AddDialog(new TruckDialog());
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
            var promptMessage = MessageFactory.Text("Informe seu RENAVAM", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //private async Task<DialogTurnResult> ValidatorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
        //        return await stepContext.EndDialogAsync(cancellationToken);

        //}


        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            cardDialogDetails = (CardDialogDetails)stepContext.Options;
            cardDialogDetails.Renavam = stepContext.Result.ToString();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
            string messagetext = null;
            var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        }

        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            cardDialogDetails = (CardDialogDetails)stepContext.Options;
            cardDialogDetails.SecureCode = stepContext.Result.ToString();

            var renv = stepContext.Result.ToString();

            switch (stepContext.Result.ToString())
            {
                case "1111":
                    return await stepContext.BeginDialogAsync(nameof(CarDialog), cardDialogDetails, cancellationToken);
                case "2222":
                    return await stepContext.BeginDialogAsync(nameof(TruckDialog), cardDialogDetails, cancellationToken);
                default:
                    return await stepContext.BeginDialogAsync(nameof(BaneseLicenseDialog), cardDialogDetails, cancellationToken);
            }
        }



        //private static async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var info = "Aqui está o sua via para pagamento no BANESE!\r\n" +
        //               "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
        //               " - PDF\r\n" +
        //               " - Código de Barras: 00001222 222525 56599595 5544444";
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
        //    return await stepContext.EndDialogAsync(cancellationToken);
        //}

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var info = "Aqui está o sua via para pagamento em OUTROS BANCOS!\r\n" +
                       "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                       "(Compensação em 4 dias úteis e custo adicional de R$ 2,00)\r\n";

            var code = "Código de Barras: 00001222 222525 56599595 5544444";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(code), cancellationToken);


            // Define choices
            var choices = new[] { "Baixar PDF" };

            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = choices.Select(choice => new AdaptiveOpenUrlAction
                {
                    Title = choice,
                    Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

                }).ToList<AdaptiveAction>(),
            };

            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(choices),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
                cancellationToken);
        }


    }

}
