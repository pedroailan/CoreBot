// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using CoreBot.Fields;
using CoreBot.Models.Methods;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            CRLVDialogDetails = (CRLVDialogDetails)stepContext.Options;
            CRLVDialogDetails.codSegurancaIn = stepContext.Result.ToString();

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o código de segurança informado. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);


            if (await VehicleCRLV.ValidationSecureCode(CRLVDialogDetails.codSegurancaIn) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVDialogDetails, cancellationToken);
            }
            // Caso o Código de Segurança não seja válido
            else
            {
                switch (CRLVDialogDetails.Erro.codigo == 1 ? "Incorreto" :
                           CRLVDialogDetails.Erro.codigo >= 2 && CRLVDialogDetails.Erro.codigo <= 900 ? "Sistema" :
                           CRLVDialogDetails.Erro.codigo == 0 && Format.Input.ValidationFormat.IsNumber(CRLVDialogDetails.codSegurancaIn) ? "Conexao" :
                           CRLVDialogDetails.Erro.codigo == 0 && !Format.Input.ValidationFormat.IsNumber(CRLVDialogDetails.codSegurancaIn) ? "Invalido" : null)
                {
                    case "Incorreto":
                        await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);

                        CRLVDialogDetails.Count += 1;
                        if (CRLVDialogDetails.Count < 3)
                        {
                            CRLVDialogDetails.Erro.codigo = 0;
                            return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), CRLVDialogDetails, cancellationToken);
                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança\r\n" +
                                                                        "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                        "ou entre em contato com o DETRAN, para obter mais informações");
                            return await stepContext.EndDialogAsync(cancellationToken);
                        }
                    case "Sistema":
                        await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    case "Conexao":
                        await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                    ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                        return await stepContext.EndDialogAsync(cancellationToken);
                    case "Invalido":
                        await stepContext.Context.SendActivityAsync("Este código de segurança é inválido!");
                        CRLVDialogDetails.Count += 1;
                        if (CRLVDialogDetails.Count < 3)
                        {
                            return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), CRLVDialogDetails, cancellationToken);
                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança!\r\n" +
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
    }
}
