// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-PT"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RootCRLVeDialog());
            AddDialog(new RootLicenseDialog());
            AddDialog(new RootOthersServicesDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                AskStepAsync,
                FinalStepAsync,
                //AvaliationStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //ObterEmissaoCRLV novo = new ObterEmissaoCRLV();
            ////var algo = await novo.obterEmissaoCRLV("OEK8190");
            //var algo = await novo.obterEmissaoCRLV("OEK8190", 50188645808);
            ///*var algo = await novo.obterEmissaoCRLV("", 0);*/

            //LicenseDialogDetails.MarcaModelo = algo.nomeProprietario;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Olá, posso ajudá-lo com alguma das opções abaixo? "),
                RetryPrompt = MessageFactory.Text("Por favor, insira uma das opções abaixo:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Licenciamento Anual (BANESE)", "Licenciamento Anual (Outros Bancos)", "Emitir Documento de Circulação (CRLV-e)", "Nenhuma das alternativas" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            var LicenseDialogDetails = new LicenseDialogDetails();
            var CRLVDialogDetails = new CRLVDialogDetails();
            var LicenseFields = new LicenseFields();
            var CRLVeFields = new CRLVeFields();

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            stepContext.FindDialog(nameof(RootLicenseDialog));

            switch (stepContext.Values["choice"].ToString().ToLower())
            {
                case "nenhuma das alternativas":
                    return await stepContext.BeginDialogAsync(nameof(RootOthersServicesDialog), LicenseFields, cancellationToken);
                case "licenciamento anual (banese)":
                    LicenseFields.tipoDocumentoIn = "D";
                    LicenseFields.Banco = "Banese";
                    return await stepContext.BeginDialogAsync(nameof(RootLicenseDialog), LicenseFields, cancellationToken);
                case "licenciamento anual (outros bancos)":
                    LicenseFields.tipoDocumentoIn = "F";
                    LicenseFields.Banco = "Outros Bancos";
                    return await stepContext.BeginDialogAsync(nameof(RootLicenseDialog), LicenseFields, cancellationToken);
                case "emitir documento de circulação (crlv-e)":
                    return await stepContext.BeginDialogAsync(nameof(RootCRLVeDialog), CRLVeFields, cancellationToken);
                default:
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Por favor, insira uma das opções acima")
                    };

                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog),cancellationToken);
            }


            /*var officeLessDetails = new OfficeLessDetails();
            officeLessDetails.Nome = stepContext.Result.ToString();
            if (!string.IsNullOrEmpty(officeLessDetails.Nome)) {
               return await stepContext.BeginDialogAsync(nameof(OfficeLessDialog), officeLessDetails, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                //return await stepContext.NextAsync(-1, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }*/

        }

        private async Task<DialogTurnResult> AskStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivitiesAsync(new Activity[]
           {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value = 1000},
           }, cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Ajuda + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Ajuda + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não"}),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if(stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), cancellationToken);
            } else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(TextGlobal.Agradecimento), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                //var promptOptions = new PromptOptions
                //{
                //    Prompt = MessageFactory.Text($"De 1 a 5, qual nota você daria para meu atendimento?"),
                //    Choices = ChoiceFactory.ToChoices(new List<string> { "1 - Péssimo", "2 - Ruim", "3 - Regular", "4 - Bom", "5 - Excelente" }),
                //};
                //return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }

            //if (stepContext.Result != null)
            //{
            //    var result = stepContext.Result;
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.ToString()), cancellationToken);
            //}
            //else
            //{
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigada."), cancellationToken);
            //}
        }

        private async Task<DialogTurnResult> AvaliationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(TextGlobal.Agradecimento), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
