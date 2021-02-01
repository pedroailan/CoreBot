// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootCRLVeDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public RootCRLVeDialog()
            : base(nameof(RootCRLVeDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TruckDialog());
            AddDialog(new PlacaDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new SecureCodeCRLVeDialog());
      
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                SecureCodeQuestionStepAsync,
                SecureCodeStepAsync,
                //ValidationSecureCodeStepAsync,
                //SendSecureCodeStepAsync
                //RenavamStepAsync,
                //ValidatorStepAsync
                //FinalStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            await stepContext.Context.SendActivityAsync("Bem-vindo ao serviço de Licenciamento Anual!");
            await stepContext.Context.SendActivityAsync("Aqui você pode emitir o Documento de Circulação de porte obrigatório (CRVL-e).\r\n");
            
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Deseja prosseguir?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {

                return await stepContext.ContinueDialogAsync(cancellationToken);

            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog));
            }
        }


        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Para iniciarmos o processo de emissão do Documento de Circulação de Porte Obrigatório (CRLV-e), vou precisar de algumas informações.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(text), cancellationToken);

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
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
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
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {

                return await stepContext.BeginDialogAsync(nameof(SecureCodeCRLVeDialog), LicenseDialogDetails, cancellationToken);

            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), LicenseDialogDetails, cancellationToken);
            }
        }


        //private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
        //    LicenseDialogDetails.Renavam = stepContext.Result.ToString();

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Agora, informe o código de segurança"), cancellationToken);
        //    string messagetext = null;
        //    var secureCode = MessageFactory.Text(messagetext, InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);

        //}

        //private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
        //    LicenseDialogDetails.SecureCode = stepContext.Result.ToString();

        //    var renv = stepContext.Result.ToString();

        //    switch (stepContext.Result.ToString())
        //    {
        //        case "50515253545":
        //            return await stepContext.BeginDialogAsync(nameof(CarDialog), LicenseDialogDetails, cancellationToken);
        //        case "49505152535":
        //            return await stepContext.BeginDialogAsync(nameof(TruckDialog), LicenseDialogDetails, cancellationToken);
        //        default:
        //            return await stepContext.BeginDialogAsync(nameof(BaneseLicenseDialog), LicenseDialogDetails, cancellationToken);
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
