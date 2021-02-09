using CoreBot.Services.Fields;
using CoreBot.Services.ServiceLicenciamento;
using Microsoft.BotBuilderSamples;
using Refit;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace CoreBot.Services.ValidationServiceLicenciamento
{
    public class Consumer1
    {
        public static async Task ConsumerAsync()
        {
            var inFields = new InFields();
            try
            {
                var codRenavam = RestService.For<IApiServiceVD>("http://172.28.64.58:8176/wsChatbot?wsdl");

                var info = await codRenavam.VServiceLicenciamento(inFields.codSeguranca, inFields.renavam, inFields.tipoDocumentoIn, inFields.anoLicenciamento);
            }
            catch (Exception e)
            {
                LicenseDialogDetails.Error = e.Message;
            }
        }

        public static async Task ConsumerAsyncXML()
        {
            String URLString = "http://172.28.64.58:9176/homologa/serviceChatBot?wsdl";
            XmlTextReader reader = new XmlTextReader (URLString);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        
                        if (reader.HasAttributes)
                        {
                            //Console.WriteLine("Attributes of <" + reader.Name + ">");
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {   if (reader.Name == "wsdl:message")
                                {
                                    LicenseDialogDetails.MarcaModelo = reader.LocalName;
                                    LicenseDialogDetails.Placa = reader.LineNumber.ToString();
                                    LicenseDialogDetails.NomeProprietario = reader.BaseURI;
                                    return;
                                }
                            }
                            // Move the reader back to the element node.
                            reader.MoveToElement();
                        }

                        while (reader.MoveToNextAttribute()) // Read the attributes.
                            //LicenseDialogDetails.Placa = (" " + reader.Name + "='" + reader.Value + "'");
                        Console.Write(">");
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine (reader.Value);
                        break;
                    case XmlNodeType. EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            
        }
    }
}
