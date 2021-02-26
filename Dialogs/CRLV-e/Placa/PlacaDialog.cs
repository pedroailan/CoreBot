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
using CoreBot.Models.Methods;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PlacaDialog : CancelAndHelpDialog
    {

        private CRLVeFields CRLVeFields;

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
                ValidationPlacaStepAsync,
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

            CRLVeFields = (CRLVeFields)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe a PLACA do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationPlacaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Options;

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando a placa informada. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            CRLVeFields.placaIn = stepContext.Result.ToString();

            VehicleCRLV vehicle = new VehicleCRLV();

            if (vehicle.ValidationStringPlaca(CRLVeFields.placaIn) == true)
            {
                var webResult = await vehicle.ValidationPlaca(CRLVeFields.placaIn);

                CRLVeFields.codigoRetorno = webResult.codigoRetorno;
                CRLVeFields.codSegurançaOut = webResult.codSegurancaOut.ToString();
                CRLVeFields.renavam = webResult.renavam.ToString();
                CRLVeFields.nomeProprietario = webResult.nomeProprietario;
                CRLVeFields.placaOut = webResult.placaOut;
                CRLVeFields.codigoRetorno = webResult.codigoRetorno;
                CRLVeFields.documentoCRLVePdf = webResult.documentoCRLVePdf;
                CRLVeFields.erroCodigo = webResult.erro.codigo;
                CRLVeFields.erroMensagem = webResult.erro.mensagem;
                CRLVeFields.erroTrace = webResult.erro.trace;


                if (CRLVeFields.erroCodigo == 1)
                {
                    await stepContext.Context.SendActivityAsync("Erro: " + CRLVeFields.erroMensagem);

                    CRLVeFields.Count += 1;
                    if (CRLVeFields.Count < 3)
                    {
                        CRLVeFields.erroCodigo = 0;
                        return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), CRLVeFields, cancellationToken);
                    }
                    else
                    {
                        await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança\r\n" +
                                                                    "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                    "ou entre em contato com o DETRAN, para obter mais informações");
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }
                }
                // Caso erro 2 <= x <= 900
                else if (CRLVeFields.erroCodigo >= 2 && CRLVeFields.erroCodigo <= 900)
                {
                    await stepContext.Context.SendActivityAsync("Erro: " + CRLVeFields.erroMensagem);
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
                // Caso retorne nenhum erro, mas tenha falha na conexao
                else if (CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 0)
                {
                    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
                // Caso não haja erros
                else if (CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 1)
                {
                    if (VehicleCRLV.ExistSecureCodePlaca(CRLVeFields.codSegurançaOut) == true)
                    {
                        //CRLVeFields.secureCodeBool = true;
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
                            Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
                            RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
                            Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                        };
                        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
                    }
                    else
                    {
                        return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVeFields, cancellationToken);
                    }

                    //return await stepContext.BeginDialogAsync(nameof(PlacaDialog), CRLVeFields, cancellationToken);
                }
                // Erro crítico (Sistema fora)
                else
                {
                    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }
            else
            {
                // Se a string for inválida
                await stepContext.Context.SendActivityAsync("Esta placa é inválida!");
                CRLVeFields.Count += 1;
                if (CRLVeFields.Count < 3)
                {
                    return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), CRLVeFields, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar a placa!\r\n" +
                                                                "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                                "ou entre em contato com o DETRAN, para obter mais informações");
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }

            //if (await VehicleCRLV.ValidationPlaca(CRLVDialogDetails.placaIn) == true) // Validação da placa
            //{
            //    if (VehicleLicense.Situation(CRLVDialogDetails.placaIn) == true) // Caso possua alguma pendência, por hora não se aplica
            //    {
            //        if (VehicleCRLV.ExistSecureCodePlaca() == true)
            //        {
            //            //CRLVDialogDetails.secureCodeBool = true;
            //            await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, vou precisar dessa informação");
            //            AdaptiveCard card = new AdaptiveCard("1.0")
            //            {
            //                Body =
            //                {
            //                    new AdaptiveImage()
            //                    {
            //                        Type = "Image",
            //                        Size = AdaptiveImageSize.Auto,
            //                        Style = AdaptiveImageStyle.Default,
            //                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
            //                        Separator = true,
            //                        Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
            //                    }
            //                }
            //            };

            //            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
            //            {
            //                Content = card,
            //                ContentType = "application/vnd.microsoft.card.adaptive",
            //                Name = "cardName"
            //            }
            //            ), cancellationToken);

            //            var promptOptions = new PromptOptions
            //            {
            //                Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
            //                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
            //                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            //            };
            //            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            //        }
            //        else
            //        {
            //            return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVDialogDetails, cancellationToken);
            //        }
            //    }
            //    else
            //    {
            //        await stepContext.Context.SendActivityAsync("Veículo não autorizado: motivo(s)");
            //        return await stepContext.EndDialogAsync(cancellationToken);
            //    }
            //}
            //// Caso a placa não seja válida
            //else
            //{
            //    switch (CRLVDialogDetails.Erro.codigo == 1 ? "Incorreto" :
            //            CRLVDialogDetails.Erro.codigo >= 2 && CRLVDialogDetails.Erro.codigo <= 900 ? "Sistema" : null)
            //    {
            //        case "Incorreto":
            //            await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);

            //            CRLVDialogDetails.Count += 1;
            //            if (CRLVDialogDetails.Count < 3)
            //            {
            //                CRLVDialogDetails.Erro.codigo = 0;
            //                return await stepContext.ReplaceDialogAsync(nameof(SecureCodeCRLVeDialog), CRLVDialogDetails, cancellationToken);
            //            }
            //            else
            //            {
            //                await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar o código de segurança\r\n" +
            //                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
            //                                                            "ou entre em contato com o DETRAN, para obter mais informações");
            //                return await stepContext.EndDialogAsync(cancellationToken);
            //            }
            //        case "Sistema":
            //            await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);
            //            return await stepContext.EndDialogAsync(cancellationToken);
            //        default:
            //            await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                        ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //            return await stepContext.EndDialogAsync(cancellationToken);
            //    }

            //if (CRLVDialogDetails.Erro.codigo == 1)
            //{
            //    await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);

            //    CRLVDialogDetails.Count += 1;
            //    if (CRLVDialogDetails.Count < 3)
            //    {
            //        return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), CRLVDialogDetails, cancellationToken);
            //    }
            //    else
            //    {
            //        await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar a Placa\r\n" +
            //                                                    "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
            //                                                    "ou entre em contato com o DETRAN, para obter mais informações");
            //        return await stepContext.EndDialogAsync(cancellationToken);
            //    }
            //}
            //else if (CRLVDialogDetails.Erro.codigo >= 2 && CRLVDialogDetails.Erro.codigo <= 900)
            //{
            //    await stepContext.Context.SendActivityAsync("Erro: " + CRLVDialogDetails.Erro.mensagem);
            //    return await stepContext.EndDialogAsync(cancellationToken);
            //}
            //else
            //{
            //    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
            //                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
            //    return await stepContext.EndDialogAsync(cancellationToken);
            //}
            //}
        }

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                // SecureCodeDialog
                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeCRLVeDialog), CRLVeFields, cancellationToken);

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