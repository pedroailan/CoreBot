// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Generate;
using CoreBot.Models.Generate.Converter;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationsCRLVeDialog : CancelAndHelpDialog
    {

        private CRLVDialogDetails CRLVDialogDetails;


        public SpecificationsCRLVeDialog()
            : base(nameof(SpecificationsCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                ConfirmDataAsync,
                FinalStepAsync,
                //DownloadStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Renavam: " + CRLVDialogDetails.renavam +
                                                                            "\r\nPlaca: " + CRLVDialogDetails.placaOut +
                                                                            "\r\nProprietário: " + CRLVDialogDetails.nomeProprietario),
                                                                            cancellationToken);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Seus dados estão corretos?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Ok, teremos que repetir o processo.");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), CRLVDialogDetails, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(CRLVDialogDetails.codigoRetorno == 1) {
                await stepContext.Context.SendActivityAsync("Aqui está o seu Documento de Circulação de Porte Obrigatório (CRLV-e)! Para baixar basta clicar no item abaixo.");
                await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(
                        PdfProvider.Disponibilizer(CRLVDialogDetails.documentoCRLVePdf, "CRLVe_" + CRLVDialogDetails.codSegurançaOut)
                    )
                }, cancellationToken);
            } else
            {
                await stepContext.Context.SendActivityAsync("Ocorreu um erro no processamento do PDF, tente novamente mais tarde.");
            }
            

            //var reply = MessageFactory.Text("Aqui está o seu Documento de Circulação de Porte Obrigatório (CRLV-e).");
            //reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(CRLVDialogDetails.documentoCRLVePdf, "CRLVe_" + CRLVDialogDetails.codSegurançaOut) };
            //await stepContext.Context.SendActivityAsync(reply);

            return await stepContext.EndDialogAsync(cancellationToken);


            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Este é seu Documento de Circulação de Porte Obrigatório (CRLV-e)!"));

            //AdaptiveCard card = new AdaptiveCard("1.0")
            //{
            //    Body =
            //        {
            //            new AdaptiveImage()
            //            {
            //                Type = "Image",
            //                Size = AdaptiveImageSize.Auto,
            //                Style = AdaptiveImageStyle.Default,
            //                HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
            //                Separator = true,
            //                Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
            //            }
            //        }
            //};

            //await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
            //{
            //    Content = card,
            //    ContentType = "application/vnd.microsoft.card.adaptive",
            //    Name = "cardName"
            //}
            //), cancellationToken);


            //var promptOptions = new PromptOptions
            //{
            //    Prompt = MessageFactory.Text($"Baixar Documento de Circulação de Porte Obrigatório (CRLV-e)?"),
            //    Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
            //};

            //return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

            // --------------------------------------------------------------------------------------------------------------

            // Define choices
            //var choices = new[] { "Baixar Documento de Circulação de Porte Obrigatório (CRLV-e)" };

            //// Create card
            //var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            //{
            //    // Use LINQ to turn the choices into submit actions
            //    Body =
            //    {
            //        new AdaptiveImage()
            //        {
            //            Type = "Image",
            //            Size = AdaptiveImageSize.Auto,
            //            Style = AdaptiveImageStyle.Default,
            //            HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
            //            Separator = true,
            //            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
            //        }
            //    },
            //    Actions = choices.Select(choice => new AdaptiveOpenUrlAction
            //    {
            //        Title = choice,
            //        Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1"),
            //    }).ToList<AdaptiveAction>(),
            //};

            //// Prompt
            //await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            //{
            //    Prompt = (Activity)MessageFactory.Attachment(new Attachment
            //    {
            //        ContentType = AdaptiveCard.ContentType,
            //        // Convert the AdaptiveCard to a JObject
            //        Content = JObject.FromObject(card),
            //    }),
            //    Choices = ChoiceFactory.ToChoices(choices),
            //    // Don't render the choices outside the card
            //    Style = ListStyle.None,
            //},
            //    cancellationToken);

            //return await stepContext.EndDialogAsync(cancellationToken);
        }

        //private async Task<DialogTurnResult> DownloadStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
            
        //    stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
        //    if (stepContext.Values["choice"].ToString().ToLower() == "sim")
        //    {
        //        ConverterStringToPDF.converter(CRLVDialogDetails.documentoCRLVePdf);
        //        var reply = MessageFactory.Text("Aqui está o seu documento");
        //        reply.Attachments = new List<Attachment>() { pdf() };
        //        await stepContext.Context.SendActivityAsync(reply);
        //        return await stepContext.EndDialogAsync(cancellationToken);
        //    }
        //    else
        //    {
        //        return await stepContext.EndDialogAsync(cancellationToken);
        //    }
        //}

        //private static Attachment GetInlineAttachment()
        //{
        //    var imagePath = Path.Combine(Environment.CurrentDirectory, @"Assets/App", "icon_pdf_download.png");
        //    var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

        //    return new Attachment
        //    {
        //        Name = @"Assets\App\icon_pdf_download.png",
        //        ContentType = "image/png",
        //        ContentUrl = $"data:image/png;base64,{imageData}",
        //    };
        //}

    }
}
