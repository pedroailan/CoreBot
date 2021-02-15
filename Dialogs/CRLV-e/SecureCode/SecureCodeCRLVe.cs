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
using CoreBot.Fields;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SecureCodeCRLVeDialog : CancelAndHelpDialog
    {

        private CRLVDialogDetails CRLVDialogDetails;

        public SecureCodeCRLVeDialog()
            : base(nameof(SecureCodeCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationsCRLVeDialog());
            AddDialog(new RequiredSecureCodeCRLVeDialog());
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

            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        /// <summary>
        /// OBJETIVO: 
        ///     Realizar a primeira validação do código de segurança
        ///     Se for verdade, as Fields do CRLV já estarão preenchidas em tempo de execução
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando seu código de segurança, aguarde um momento."),
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value= 8000 },
                MessageFactory.Text(""),

           }, cancellationToken);

            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;
            CRLVDialogDetails.codSegurancaIn = stepContext.Result.ToString();

            if (await Vehicle.ValidationSecureCode(CRLVDialogDetails.codSegurancaIn) == true)
            {            
                return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVDialogDetails, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(CRLVDialogDetails.Erro.mensagem), cancellationToken);

                CRLVDialogDetails.Count += 1;
                if (CRLVDialogDetails.Count < 3)
                {
                    await stepContext.Context.SendActivityAsync("Este CÓDIGO DE SEGURANÇA é inválido!");
                    return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), CRLVDialogDetails, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança\r\n" +
                                                                "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois\r\n" +
                                                                "ou entre em contato com o DETRAN, para obter mais informações");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }

        }
    }
}
