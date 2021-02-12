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
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Extensions.Options;
using CoreBot;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SecureCodeDialog : CancelAndHelpDialog
    {

        private LicenseDialogDetails LicenseDialogDetails;

        public SecureCodeDialog()
            : base(nameof(SecureCodeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationsDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeRequiredStepAsync,
                VerificationSecureCodeStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SecureCodeRequiredStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.codSegurancaIn = stepContext.Result.ToString();
          
            if (await Vehicle.ValidationSecureCodeLicenciamento(LicenseDialogDetails.codSegurancaIn) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                if (Vehicle.Situation(LicenseDialogDetails.placa) == true)
                {
                    await stepContext.Context.SendActivityAsync("Este CÓDIGO DE SEGURANÇA é inválido!");
                    if (LicenseDialogDetails.SecureCodeBool == true || LicenseDialogDetails.Count < 3)
                    {
                        LicenseDialogDetails.Count += 1;
                        if (LicenseDialogDetails.Count < 3)
                        {
                            return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseDialogDetails, cancellationToken);
                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança!\r\n" +
                                                                        "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                        "ou entre em contato com o DETRAN, para obter mais informações");
                            return await stepContext.EndDialogAsync(cancellationToken);
                        }
                    }
                    else
                    {
                        return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseDialogDetails, cancellationToken);
                    }
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("{Informa o motivo ao cliente}");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
                
                
            }
        }

    }
}
