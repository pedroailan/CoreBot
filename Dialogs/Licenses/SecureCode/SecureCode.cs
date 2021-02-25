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
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Fields;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SecureCodeDialog : CancelAndHelpDialog
    {

        //private LicenseDialogDetails LicenseDialogDetails;
        LicenseFields LicenseFields;
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

            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;

            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;
            LicenseFields.codSegurancaIn = stepContext.Result.ToString();

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o código de segurança informado. Por favor, aguarde um momento..." + Emojis.Rostos.Sorriso),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            VehicleLicense vehicle = new VehicleLicense();

            if (vehicle.ValidationString(LicenseFields.codSegurancaIn) == true)
            {
                var webResult = await vehicle.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaIn, LicenseFields.tipoDocumentoIn);

                LicenseFields.codigoRetorno = webResult.codigoRetorno;
                //LicenseFields.Erro erro = new LicenseFields.Erro();
                LicenseFields.erroCodigo = webResult.erro.codigo;
                LicenseFields.erroMensagem = webResult.erro.mensagem;
                LicenseFields.erroTrace = webResult.erro.trace;
                LicenseFields.codSegurancaOut = webResult.codSegurancaOut.ToString();
                LicenseFields.renavamOut = webResult.renavamOut.ToString();
                LicenseFields.placa = webResult.placa;
                LicenseFields.nomeProprietario = webResult.nomeProprietario;
                LicenseFields.temRNTRC = webResult.temRNTRC;
                LicenseFields.tipoAutorizacaoRNTRCOut = webResult.tipoAutorizacaoRNTRC;
                LicenseFields.nroAutorizacaoRNTRCOut = webResult.nroAutorizacaoRNTRC;
                LicenseFields.temIsençãoIPVA = webResult.temIsencaoIPVA;
                LicenseFields.restricao = webResult.restricao;
                LicenseFields.anoLicenciamento = webResult.anoLicenciamento;
                LicenseFields.totalCotaUnica = webResult.totalCotaUnica;
                LicenseFields.contadorAnoLicenciamento = webResult.contadorAnoLicenciamento;
                LicenseFields.recallCodigo = webResult.recallPendente.codigo;
                LicenseFields.recallMensagem = webResult.recallPendente.mensagem;
                LicenseFields.recallDescricao = new string[] { webResult.recallPendente.listaRecall.ToString() };

                if (LicenseFields.erroCodigo == 1)
                {
                    await stepContext.Context.SendActivityAsync("Erro: " + LicenseFields.erroMensagem);
                    if (LicenseFields.SecureCodeBool == true || LicenseFields.Count < 3)
                    {
                        LicenseFields.Count += 1;
                        if (LicenseFields.Count < 3)
                        {
                            LicenseFields.erroCodigo = 0;
                            return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
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
                        return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
                    }
                } 
                // Caso erro 2 <= x <= 900
                else if (LicenseFields.erroCodigo >= 2 && LicenseFields.erroCodigo <= 900)
                {
                    await stepContext.Context.SendActivityAsync("Erro: " + LicenseFields.erroMensagem);
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
                //else if (erro.codigo == 0)
                //{
                //    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                //                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                //    return await stepContext.EndDialogAsync(cancellationToken);
                //}
                // Caso não haja erros
                else
                {
                    return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
                }

            }
            else
            {
                // Se a string for inválida
                await stepContext.Context.SendActivityAsync("Este código de segurança é inválido!");
                LicenseFields.Count += 1;
                if (LicenseFields.Count < 3)
                {
                    return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança!\r\n" +
                                                                "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                "ou entre em contato com o DETRAN, para obter mais informações");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }



            // Caso o código de Segurança seja inválido
            //else
            //{

            //    if (VehicleLicense.Situation(LicenseFields.placa) == true)
            //    {
            //        switch (erro.codigo == 1 ? "Incorreto" :
            //               erro.codigo >= 2 && erro.codigo <= 900 ? "Sistema" :
            //               erro.codigo == 0 && Format.Input.ValidationFormat.IsNumber(LicenseFields.codSegurancaIn) ? "Conexao" :
            //               erro.codigo == 0 && !Format.Input.ValidationFormat.IsNumber(LicenseFields.codSegurancaIn) ? "Invalido" : null)
            //        {
            //            case "Incorreto":
            //                await stepContext.Context.SendActivityAsync("Erro: " + erro.mensagem);
            //                if (LicenseFields.SecureCodeBool == true || LicenseFields.Count < 3)
            //                {
            //                    LicenseFields.Count += 1;
            //                    if (LicenseFields.Count < 3)
            //                    {
            //                        erro.codigo = 0;
            //                        return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
            //                    }
            //                    else
            //                    {
            //                        await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança!\r\n" +
            //                                                                    "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
            //                                                                    "ou entre em contato com o DETRAN, para obter mais informações");
            //                        return await stepContext.EndDialogAsync(cancellationToken);
            //                    }
            //                }
            //                else
            //                {
            //                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
            //                }
            //            case "Sistema":
            //                await stepContext.Context.SendActivityAsync("Erro: " + erro.mensagem);
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //            case "Conexao":
            //                await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                            ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //            case "Invalido":
            //                await stepContext.Context.SendActivityAsync("Este código de segurança é inválido!");
            //                LicenseFields.Count += 1;
            //                if (LicenseFields.Count < 3)
            //                {
            //                    return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
            //                }
            //                else
            //                {
            //                    await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança!\r\n" +
            //                                                                "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
            //                                                                "ou entre em contato com o DETRAN, para obter mais informações");
            //                    return await stepContext.EndDialogAsync(cancellationToken);
            //                }
            //            default:
            //                await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                            ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //        }
            //    }
            //    else
            //    {
            //        await stepContext.Context.SendActivityAsync("{Informa o motivo ao cliente}");
            //        return await stepContext.EndDialogAsync(LicenseFields, cancellationToken);
            //    }
            //}
        }
    }
}
