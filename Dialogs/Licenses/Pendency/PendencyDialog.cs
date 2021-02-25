// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PendencyDialog : CancelAndHelpDialog
    {
        //private LicenseFields LicenseFields;
        LicenseFields LicenseFields;
         
        public PendencyDialog()
            : base(nameof(PendencyDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            //AddDialog(new SpecificationsDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PendencyStepAsync,
                ConfirmValidationStepAsync,
                Pendency_2StepAsync,
                Pendency_3StepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        class ano
        {
            public static string anoAnterior = ((DateTime.Now.Year) - 1).ToString();
            public static string anoAtual = DateTime.Now.Year.ToString();
        }
        private async Task<DialogTurnResult> PendencyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //LicenseFields = (LicenseFields)stepContext.Options;
            LicenseFields = (LicenseFields)stepContext.Options;


            if (VehicleLicense.ValidationYear() == true)
            {
                await stepContext.Context.SendActivityAsync("Detectei também que você pode optar por licenciar o ano anterior");
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);
                //return await stepContext.BeginDialogAsync(nameof(PendencyDialog), LicenseFields, cancellationToken);
            }
            else
            {
                //await stepContext.Context.SendActivityAsync("Valor a ser pago:");
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);
                LicenseFields.exercicio = LicenseFields.anoLicenciamento[0];

                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                    RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja Prosseguir?" + TextGlobal.ChoiceDig),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), Options, cancellationToken);
            }

            List<string> anos = new List<string>();
            for (int i = 0; i < LicenseFields.contadorAnoLicenciamento; i++)
            {
                if (LicenseFields.anoLicenciamento[i] != 0)
                {
                    anos.Add(LicenseFields.anoLicenciamento[i].ToString());
                }
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual ano deseja licenciar seu veículo? (A escolha do ano atual já traz acumulado o ano anterior)"),
                Choices = ChoiceFactory.ToChoices(choices: anos),
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ConfirmValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();

            //LicenseFields.contadorAnoLicenciamento = 1;

            // Confirmação para somente um ano
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivitiesAsync(new Activity[]
                {
                    MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                    //new Activity { Type = ActivityTypes.Typing },
                }, cancellationToken);

                LicenseFields.exercicio = LicenseFields.anoLicenciamento[0];

                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                if (LicenseFields.anoLicenciamento[0] == LicenseFields.exercicio && stepContext.Values["choice"].ToString().ToLower() == "não")
                {
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog));
                }
                LicenseFields.exercicio = Convert.ToInt32(stepContext.Values["choice"]);
                if (stepContext.Values["choice"].ToString().ToLower() == LicenseFields.anoLicenciamento[0].ToString())
                {
                    await stepContext.Context.SendActivitiesAsync(new Activity[]
                    {
                        MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                       //new Activity { Type = ActivityTypes.Typing },
                    }, cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
                else
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
        }

        private async Task<DialogTurnResult> Pendency_2StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;

            if (LicenseFields.exercicio == LicenseFields.anoLicenciamento[1])
            {
                await VehicleLicense.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaOut, LicenseFields.exercicio);
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);
                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Deseja prosseguir?" + TextGlobal.Choice),
                    RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja prosseguir?" + TextGlobal.ChoiceDig),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt), Options, cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

        }

        private async Task<DialogTurnResult> Pendency_3StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivitiesAsync(new Activity[]
                {
                    MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                    //new Activity { Type = ActivityTypes.Typing },
                }, cancellationToken);

                LicenseFields.exercicio = LicenseFields.exercicio;
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                LicenseFields.exercicio -= 1;
                await VehicleLicense.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaOut, LicenseFields.exercicio);
                return await stepContext.ReplaceDialogAsync(nameof(PendencyDialog), LicenseFields, cancellationToken);
            }


        }
    }
}
