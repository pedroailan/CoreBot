// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Generate;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.efetuarServicoLicenciamento;
using CoreBot.Services.WSDLService.validarServicoLicenciamento;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationsDialog : CancelAndHelpDialog
    {

        //LicenseDialogDetails LicenseDialogDetails;
        LicenseFields LicenseFields;

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
            LicenseFields = (LicenseFields)stepContext.Options;
            //dados = new LicenseDialogDetails();
            //dados = (LicenseDialogDetails)stepContext.Options;
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
                                                                            "\r\nPlaca: " + LicenseFields.placa +
                                                                            "\r\nProprietário: " + LicenseFields.nomeProprietario),
                                                                            cancellationToken);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Seus dados estão corretos?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text("Seus dados estão corretos?" +  TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Se os dados não estão corretos, teremos que repetir o processo.\r\n" +
                                                            "Caso o problema persista, entre em contato com nossa equipe de atendimento");
                return await stepContext.ReplaceDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> TypeVehicleAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            if (VehicleLicenseRNTRC.ValidationVehicleType(LicenseFields.temRNTRC) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
            }
            else
            {
                LicenseFields.dataValidadeRNTRC = "0";
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> RecallVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            if (VehicleLicenseRecall.ValidationVehicleRecall(LicenseFields.recallCodigo) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(RecallDialog), LicenseFields, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ExemptionVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields.IsencaoIPVA = "N";
            if (VehicleLicenseExemption.Exemption(LicenseFields.temIsençãoIPVA) == true)
            {
                LicenseFields.IsencaoIPVA = "S";
                return await stepContext.BeginDialogAsync(nameof(ExemptionDialog), LicenseFields, cancellationToken);
            }
            else
            {

                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }


        private async Task<DialogTurnResult> InvoiceVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            if (VehicleLicense.Pendency(LicenseFields.anoLicenciamento) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(PendencyDialog), LicenseFields, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

        }

        private async Task<DialogTurnResult> VehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            //Generate.GenerateInvoice(LicenseDialogDetails.AnoExercicio);

            //await stepContext.Context.SendActivityAsync("Você escolheu " + LicenseDialogDetails.AnoExercicio);

            var result = await EfetuarServicoLicenciamento.efeutarServicoLicenciamento(
                Convert.ToDouble(LicenseFields.renavamOut),
                Convert.ToDouble(LicenseFields.codSegurancaOut),
                LicenseFields.restricao,
                LicenseFields.exercicio,
                LicenseFields.tipoAutorizacaoRNTRCOut,
                Convert.ToDouble(LicenseFields.nroAutorizacaoRNTRCOut),
                LicenseFields.dataValidadeRNTRC,
                LicenseFields.IsencaoIPVA,
                LicenseFields.tipoDocumentoIn
                );

            //await stepContext.Context.SendActivityAsync(result.codigoRetorno.ToString());
            
            
            if (result.erro.codigo != 0)
            {
                await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema e não foi possível gerar o PDF. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                return await stepContext.EndDialogAsync(cancellationToken);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);

        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;

            var info = "Aqui está sua via para pagamento no " + LicenseFields.Banco +"!\r\n" +
                        "Estou disponibilizando seu documento ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                        "Após a compensação do pagamento você pode voltar aqui para emitir seu Documento de Circulação (CRLV-e).";

            var codeF = LicenseFields.codBarra;
            var codeD = LicenseFields.linhaDig;

            //await stepContext.Context.SendActivityAsync(LicenseDialogDetails.cpfCnpjPagador);

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text(codeF), cancellationToken);
            // await stepContext.Context.SendActivityAsync(MessageFactory.Text(codeD), cancellationToken);


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

            if (LicenseFields.tipoDocumentoOut == "F")
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfCompensacao.GenerateInvoice2(), "Ficha_de_compensacao_" + LicenseFields.codSegurancaOut) };
                await stepContext.Context.SendActivityAsync(reply);
                await stepContext.Context.SendActivityAsync(codeF);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfDUA.GenerateInvoice2(LicenseFields), "DUA_" + LicenseFields.codSegurancaOut) };
                await stepContext.Context.SendActivityAsync(reply);
                await stepContext.Context.SendActivityAsync(codeD);
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            //return await stepContext.EndDialogAsync(cancellationToken);
        }

    }
}
