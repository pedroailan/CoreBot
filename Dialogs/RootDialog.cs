// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : CancelAndHelpDialog
    {

        private CarDialogDetails carDialogDetails;

        public RootDialog()
            : base(nameof(RootDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new CarDialog());
            AddDialog(new TruckDialog());
            AddDialog(new SecureCodeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeQuestionStepAsync,
                SecureCodeStepAsync,
                //ValidationSecureCodeStepAsync,
                SendSecureCodeStepAsync
                //RenavamStepAsync,
                //ValidatorStepAsync,
                //OptionStepAsync,
                //FinalStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);

            //var choices = new[] { "Baixar PDF" };
            AdaptiveCard card = new AdaptiveCard("1.0")
            {
                Body =
                    {
                        new AdaptiveImage()
                        {
                            Type = "Image",
                              Size = AdaptiveImageSize.Auto,
                              Style = AdaptiveImageStyle.Default,
                              HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                              Separator = true,
                              Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg")
                        }
                    }
            };

            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive",
                Name = "cardName"
            }
            ), cancellationToken);


            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Você pode informar o CÓDIGO DE SEGURANÇA?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        //private async Task<DialogTurnResult> ValidatorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
        //        return await stepContext.EndDialogAsync(cancellationToken);

        //}

        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
                var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"));
                //var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
        }


        private async Task<DialogTurnResult> SendSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            carDialogDetails = (CarDialogDetails)stepContext.Options;
            carDialogDetails.SecureCode = stepContext.Result.ToString();

            if (SecureCode.ValidationSecureCode(carDialogDetails.SecureCode) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(SecureCodeDialog), carDialogDetails, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            
        }






        //private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    carDialogDetails = (CarDialogDetails)stepContext.Options;
        //    carDialogDetails.Renavam = stepContext.Result.ToString();

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
        //    string messagetext = null;
        //    var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        //}

        //private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    carDialogDetails = (CarDialogDetails)stepContext.Options;
        //    carDialogDetails.SecureCode = stepContext.Result.ToString();

        //    var renv = stepContext.Result.ToString();

        //    switch (stepContext.Result.ToString())
        //    {
        //        case "50515253545":
        //            return await stepContext.BeginDialogAsync(nameof(CarDialog), carDialogDetails, cancellationToken);
        //        case "49505152535":
        //            return await stepContext.BeginDialogAsync(nameof(TruckDialog), carDialogDetails, cancellationToken);
        //        default:
        //            return await stepContext.BeginDialogAsync(nameof(BaneseLicenseDialog), carDialogDetails, cancellationToken);
        //    }
        //}




        //private static async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var info = "Aqui está o sua via para pagamento no BANESE!\r\n" +
        //               "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
        //               " - PDF\r\n" +
        //               " - Código de Barras: 00001222 222525 56599595 5544444";
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
        //    return await stepContext.EndDialogAsync(cancellationToken);
        //}


        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{

        //    var info = "Aqui está sua via para pagamento no BANESE!\r\n" +
        //                "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n";

        //    var code = "Código de Barras: 00001222 222525 56599595 5544444";

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(code), cancellationToken);


        //    // Define choices
        //    var choices = new[] { "Baixar PDF" };

        //    // Create card
        //    var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        //    {
        //        // Use LINQ to turn the choices into submit actions
        //        Actions = choices.Select(choice => new AdaptiveOpenUrlAction
        //        {
        //            Title = choice,
        //            Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

        //        }).ToList<AdaptiveAction>(),
        //    };

        //    // Prompt
        //    return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
        //    {
        //        Prompt = (Activity)MessageFactory.Attachment(new Attachment
        //        {
        //            ContentType = AdaptiveCard.ContentType,
        //            // Convert the AdaptiveCard to a JObject
        //            Content = JObject.FromObject(card),
        //        }),
        //            Choices = ChoiceFactory.ToChoices(choices),
        //            // Don't render the choices outside the card
        //            Style = ListStyle.None,
        //        },
        //        cancellationToken);
        //}
    }
    
}
       