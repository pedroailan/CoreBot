// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Models;
using CoreBot.Models.Generate;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.Models;
using CoreBot.Services.ValidationServiceLicenciamento;
using CoreBot.Services.WSDLService.efetuarServicoLicenciamento;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationsDialog : CancelAndHelpDialog
    {

        LicenseDialogDetails LicenseDialogDetails;
        

        public SpecificationsDialog()
            : base(nameof(SpecificationsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new RecallDialog());
            AddDialog(new ExemptionDialog());
            AddDialog(new PendencyDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                ConfirmDataAsync,
                TypeVehicleAsync,
                RecallVehicleStepAsync,
                ExemptionVehicleStepAsync,
                InvoiceVehicleStepAsync,
                VehicleStepAsync,
                FinalStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        

        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //if (Vehicle.ValidationVehicleType() == true)
            //{
            //    LicenseDialogDetails.Vehicle = "Caminhão";
            //} else
            //{
            //    LicenseDialogDetails.Vehicle = "Carro";
            //}
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(/*$"Marca/Modelo: " + LicenseDialogDetails.marcaModelo*/
                                                                            "\r\nPlaca: " + LicenseDialogDetails.placa +
                                                                            "\r\nProprietário: " + LicenseDialogDetails.nomeProprietario),
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
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);

            }
            else
            {
                await stepContext.Context.SendActivityAsync("Beleza, vamos repetir o processo para outro veículo");
                return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseDialogDetails, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> TypeVehicleAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            if (VehicleLicenseRNTRC.ValidationVehicleType() == true)
            {
                return await stepContext.BeginDialogAsync(nameof(RNTRCDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> RecallVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            if (VehicleLicenseRecall.ValidationVehicleRecall() == true)
            {
                return await stepContext.BeginDialogAsync(nameof(RecallDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ExemptionVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseDialogDetails.IsencaoIPVA = "N";
            if (VehicleLicenseExemption.Exemption() == true)
            {
                return await stepContext.BeginDialogAsync(nameof(ExemptionDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {

                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }


        private async Task<DialogTurnResult> InvoiceVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (VehicleLicense.Pendency() == true)
            {
                return await stepContext.BeginDialogAsync(nameof(PendencyDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

        }

        private async Task<DialogTurnResult> VehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            

            //Generate.GenerateInvoice(LicenseDialogDetails.AnoExercicio);

            //await stepContext.Context.SendActivityAsync("Você escolheu " + LicenseDialogDetails.AnoExercicio);

            var result = await EfetuarServicoLicenciamento.efeutarServicoLicenciamento(
                Convert.ToDouble(LicenseDialogDetails.renavamOut),
                Convert.ToDouble(LicenseDialogDetails.codSegurancaOut),
                LicenseDialogDetails.restricao,
                LicenseDialogDetails.exercicio,
                LicenseDialogDetails.tipoAutorizacaoRNTRCOut,
                Convert.ToDouble(LicenseDialogDetails.nroAutorizacaoRNTRCOut),
                LicenseDialogDetails.dataValidadeRNTRC,
                LicenseDialogDetails.IsencaoIPVA,
                LicenseDialogDetails.tipoDocumentoIn
                );
            
            if (result.erro.codigo != 0)
            {
                await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                return await stepContext.EndDialogAsync(cancellationToken);
            }

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);

        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            var info = "Aqui está sua via para pagamento no " + LicenseDialogDetails.Banco +"!\r\n" +
                        "Estou disponibilizando em formato .pdf ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                        "Após a compensação do pagamento você pode voltar aqui para emitir seu documento de circulação (CRLV-e).";

            var code = LicenseDialogDetails.codBarra;

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text(info), cancellationToken);
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text(code), cancellationToken);


            //// Define choices
            //var choices = new[] { "Baixar Documento de Arrecadação (DUA)" };

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
            //        Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

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

            if (LicenseDialogDetails.tipoDocumentoOut == "F")
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfCompensacao.GenerateInvoice2(), "Ficha_de_compensacao_" + LicenseDialogDetails.codSegurancaOut) };
                await stepContext.Context.SendActivityAsync(reply);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfDUA.GenerateInvoice2(), "DUA" + LicenseDialogDetails.codSegurancaOut) };
                await stepContext.Context.SendActivityAsync(reply);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
 
        }

    }
}
