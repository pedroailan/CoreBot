// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreBot.Fields;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PlacaDialog : CancelAndHelpDialog
    {

        private CRLVDialogDetails CRLVDialogDetails;

        public PlacaDialog()
            : base(nameof(PlacaDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeCRLVeDialog());
            AddDialog(new SpecificationsCRLVeDialog());
            AddDialog(new RequiredSecureCodeCRLVeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidationRenavamStepAsync,
                OptionValidationStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
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

            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe a PLACA do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;

            CRLVDialogDetails.placaIn = stepContext.Result.ToString();

            if (await Vehicle.ValidationPlaca(CRLVDialogDetails.placaIn) == true) // Validação da placa
            {
                if (Vehicle.Situation(CRLVDialogDetails.placaIn) == true) // Caso possua alguma pendência, por hora não se aplica
                {
                    if (Vehicle.ExistSecureCodePlaca() == true)
                    {
                        //CRLVDialogDetails.secureCodeBool = true;
                        await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, vou precisar dessa informação");
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
                            Prompt = MessageFactory.Text("Você localizou?"),
                            Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
                        };
                        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
                    }
                    else
                    {
                        return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVDialogDetails, cancellationToken);
                    }
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Veículo não autorizado: motivo(s)");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Opa, Esta placa é inválida");
                return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), CRLVDialogDetails, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {

                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeCRLVeDialog), CRLVDialogDetails, cancellationToken);

            }
            else
            {
                await stepContext.Context.SendActivityAsync("Infelizmente preciso dessa informação para prosseguir. " +
                                                            "Nesse caso, será necessário entrar em contato com o nosso atendimento!");
                var choices = new[] { "Ir para o site" };
                var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    // Use LINQ to turn the choices into submit actions

                    Actions = choices.Select(choice => new AdaptiveOpenUrlAction
                    {
                        Title = choice,
                        Url = new Uri("https://www.detran.se.gov.br/portal/?pg=atend_agendamento&pCod=1")

                    }).ToList<AdaptiveAction>(),
                };

                // Prompt
                await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
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

                return await stepContext.EndDialogAsync(nameof(MainDialog));
            }
        }
    }
}
