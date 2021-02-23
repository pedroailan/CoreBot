﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Extensions.Options;
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Fields;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RenavamDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public RenavamDialog()
            : base(nameof(RenavamDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeDialog());
            AddDialog(new RequiredSecureCodeLicenseDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidationRenavamStepAsync,
                OptionValidationStepAsync,

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

            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o RENAVAM do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o Renavam informado. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            LicenseDialogDetails.renavamIn = stepContext.Result.ToString();

            if (await VehicleLicense.ValidationRenavam(LicenseDialogDetails.renavamIn) == true)
            {
                if (VehicleLicense.ExistSecureCode() == true)
                {
                    LicenseDialogDetails.SecureCodeBool = true;
                    await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, para prosseguir será necessário informá-lo.");

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
                        Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
                        RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                    };

                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
                }
                else
                {
                    return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseDialogDetails, cancellationToken);
                }
            }
            // Caso o Renavam seja inválido
            else
            {
                switch (LicenseDialogDetails.Erro.codigo == 1 ? "Incorreto" :
                           LicenseDialogDetails.Erro.codigo >= 2 && LicenseDialogDetails.Erro.codigo <= 900 ? "Sistema" :
                           LicenseDialogDetails.Erro.codigo == 0 && Format.Input.ValidationFormat.IsNumber(LicenseDialogDetails.renavamIn) ? "Conexao" :
                           LicenseDialogDetails.Erro.codigo == 0 && !Format.Input.ValidationFormat.IsNumber(LicenseDialogDetails.renavamIn) ? "Invalido" : null)
                {
                    case "Incorreto":
                        await stepContext.Context.SendActivityAsync("Erro: " + LicenseDialogDetails.Erro.mensagem);

                        LicenseDialogDetails.Count += 1;
                        if (LicenseDialogDetails.Count < 3)
                        {
                            LicenseDialogDetails.Erro.codigo = 0;
                            return await stepContext.ReplaceDialogAsync(nameof(RenavamDialog), LicenseDialogDetails, cancellationToken);
                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o numero do Renavam!\r\n" +
                                                                        "Nesse caso, vou pedir para que procure melhor e volte a falar comigo novamente depois " +
                                                                        "ou entre em contato com o DETRAN, para obter mais informações\r\nObrigada!");
                            return await stepContext.EndDialogAsync(cancellationToken);
                        }
                    case "Sistema":
                        await stepContext.Context.SendActivityAsync("Erro: " + LicenseDialogDetails.Erro.mensagem);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    case "Conexao":
                        await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                    ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                        return await stepContext.EndDialogAsync(cancellationToken);
                    case "Invalido":
                        await stepContext.Context.SendActivityAsync("Este RENAVAM é inválido!");
                        LicenseDialogDetails.Count += 1;
                        if (LicenseDialogDetails.Count < 3)
                        {
                            return await stepContext.ReplaceDialogAsync(nameof(RenavamDialog), LicenseDialogDetails, cancellationToken);
                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o Renavam!\r\n" +
                                                                        "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                        "ou entre em contato com o DETRAN, para obter mais informações");
                            return await stepContext.EndDialogAsync(cancellationToken);
                        }
                    default:
                        await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                    ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                        return await stepContext.EndDialogAsync(cancellationToken);
                }
            }
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                // SecureCodeDialog
                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeLicenseDialog), LicenseDialogDetails, cancellationToken);

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
