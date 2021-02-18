// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private LicenseDialogDetails LicenseDialogDetails;
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

        private async Task<DialogTurnResult> PendencyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;

            string anoAnterior = ((DateTime.Now.Year) - 1).ToString();
            string anoAtual = DateTime.Now.Year.ToString();


            if (VehicleLicense.ValidationYear() == true)
            {
                await stepContext.Context.SendActivityAsync("Detectei também que você pode optar por licenciar o ano anterior");
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseDialogDetails.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseDialogDetails.totalCotaUnica);
                //return await stepContext.BeginDialogAsync(nameof(PendencyDialog), LicenseDialogDetails, cancellationToken);
            }
            else
            {
                //await stepContext.Context.SendActivityAsync("Valor a ser pago:");
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseDialogDetails.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseDialogDetails.totalCotaUnica);
                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Deseja prosseguir?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), Options, cancellationToken);
            }

            List<string> anos = new List<string>();
            for (int i = 0; i < LicenseDialogDetails.contadorAnoLicenciamento; i++)
            {
                if (LicenseDialogDetails.anoLicenciamento[i] != 0)
                {
                    anos.Add(LicenseDialogDetails.anoLicenciamento[i].ToString());
                }
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Quais deseja pagar? (A escolha do ano atual já tráz acumulado o ano anterior)"),
                Choices = ChoiceFactory.ToChoices(choices: anos),
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ConfirmValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            
            //LicenseDialogDetails.contadorAnoLicenciamento = 1;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                LicenseDialogDetails.exercicio = Convert.ToInt32(stepContext.Values["choice"]);
                if (stepContext.Values["choice"].ToString().ToLower() == "2020")
                {
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
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            //stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //double[] data = new double[] { Convert.ToDouble(stepContext.Values["choice"]) };
            

            if (LicenseDialogDetails.exercicio == 2021)
            {
                await VehicleLicense.ValidationSecureCodeLicenciamento(LicenseDialogDetails.codSegurancaOut, LicenseDialogDetails.exercicio);
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseDialogDetails.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseDialogDetails.totalCotaUnica);
                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Deseja prosseguir?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "SIM", "NÃO" }),
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
            LicenseDialogDetails = (LicenseDialogDetails)stepContext.Options;
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["choice"].ToString().ToLower();
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                LicenseDialogDetails.exercicio = LicenseDialogDetails.exercicio;
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                LicenseDialogDetails.exercicio = LicenseDialogDetails.exercicio - 1;
                await VehicleLicense.ValidationSecureCodeLicenciamento(LicenseDialogDetails.codSegurancaOut, LicenseDialogDetails.exercicio);
                return await stepContext.ReplaceDialogAsync(nameof(PendencyDialog), LicenseDialogDetails, cancellationToken);
            }


        }
    }
}
