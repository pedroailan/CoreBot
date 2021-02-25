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
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Fields;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RenavamDialog : CancelAndHelpDialog
    {

        //private LicenseFields LicenseFields;
        LicenseFields LicenseFields;

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

            LicenseFields = (LicenseFields)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o RENAVAM do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o Renavam informado. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            LicenseFields.renavamIn = stepContext.Result.ToString();


            VehicleLicense vehicle = new VehicleLicense();

            if (vehicle.ValidationString(LicenseFields.renavamIn) == true)
            {
                var webResult = await vehicle.ValidationRenavam(LicenseFields.renavamIn, LicenseFields.tipoDocumentoIn);

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
                            await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o Renavam!\r\n" +
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
                    if(LicenseFields.codSegurancaOut != "0")
                    {
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
                    return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
                }

            }
            else
            {
                // Se a string for inválida
                await stepContext.Context.SendActivityAsync("Este Renavam é inválido!");
                LicenseFields.Count += 1;
                if (LicenseFields.Count < 3)
                {
                    return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o Renavam!\r\n" +
                                                                "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                "ou entre em contato com o DETRAN, para obter mais informações");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }

            //if (await VehicleLicense.ValidationRenavam(LicenseFields.renavamIn, LicenseFields.tipoDocumentoIn) == true)
            //{
            //    if (VehicleLicense.ExistSecureCode(LicenseFields.tipoDocumentoIn) == true)
            //    {
            //        LicenseFields.SecureCodeBool = true;
            //        await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, para prosseguir será necessário informá-lo.");

            //        AdaptiveCard card = new AdaptiveCard("1.0")
            //        {
            //            Body =
            //                {
            //                new AdaptiveImage()
            //                    {
            //                        Type = "Image",
            //                        Size = AdaptiveImageSize.Auto,
            //                        Style = AdaptiveImageStyle.Default,
            //                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
            //                        Separator = true,
            //                        Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
            //                    }
            //                }
            //        };

            //        await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
            //        {
            //            Content = card,
            //            ContentType = "application/vnd.microsoft.card.adaptive",
            //            Name = "cardName"
            //        }
            //        ), cancellationToken);

            //        var promptOptions = new PromptOptions
            //        {
            //            Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
            //            RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
            //            Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            //        };

            //        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            //    }
            //    else
            //    {
            //        return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
            //    }
            //}
            //// Caso o Renavam seja inválido
            //else
            //{
            //    switch (LicenseFields.Erro.codigo == 1 ? "Incorreto" :
            //               LicenseFields.Erro.codigo >= 2 && LicenseFields.Erro.codigo <= 900 ? "Sistema" :
            //               LicenseFields.Erro.codigo == 0 && Format.Input.ValidationFormat.IsNumber(LicenseFields.renavamIn) ? "Conexao" :
            //               LicenseFields.Erro.codigo == 0 && !Format.Input.ValidationFormat.IsNumber(LicenseFields.renavamIn) ? "Invalido" : null)
            //    {
            //        case "Incorreto":
            //            await stepContext.Context.SendActivityAsync("Erro: " + LicenseFields.Erro.mensagem);

            //            LicenseFields.Count += 1;
            //            if (LicenseFields.Count < 3)
            //            {
            //                LicenseFields.Erro.codigo = 0;
            //                return await stepContext.ReplaceDialogAsync(nameof(RenavamDialog), LicenseFields, cancellationToken);
            //            }
            //            else
            //            {
            //                await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o numero do Renavam!\r\n" +
            //                                                            "Nesse caso, vou pedir para que procure melhor e volte a falar comigo novamente depois " +
            //                                                            "ou entre em contato com o DETRAN, para obter mais informações\r\nObrigada!");
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //            }
            //        case "Sistema":
            //            await stepContext.Context.SendActivityAsync("Erro: " + LicenseFields.Erro.mensagem);
            //            return await stepContext.EndDialogAsync(cancellationToken);
            //        case "Conexao":
            //            await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                        ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //            return await stepContext.EndDialogAsync(cancellationToken);
            //        case "Invalido":
            //            await stepContext.Context.SendActivityAsync("Este RENAVAM é inválido!");
            //            LicenseFields.Count += 1;
            //            if (LicenseFields.Count < 3)
            //            {
            //                return await stepContext.ReplaceDialogAsync(nameof(RenavamDialog), LicenseFields, cancellationToken);
            //            }
            //            else
            //            {
            //                await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o Renavam!\r\n" +
            //                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
            //                                                            "ou entre em contato com o DETRAN, para obter mais informações");
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //            }
            //        default:
            //            await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                        ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //            return await stepContext.EndDialogAsync(cancellationToken);
            //    }
            //}
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                // SecureCodeDialog
                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeLicenseDialog), LicenseFields, cancellationToken);

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
