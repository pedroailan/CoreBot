using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO; // Entrada e saída de arquivos
using iTextSharp; // Biblioteca ITextSharp
using iTextSharp.text; // Extensão 1 - Text
using iTextSharp.text.pdf; // Extensão 2 - PDF
using iTextSharp.text.html.simpleparser;
using CoreBot.Models.Generate;

namespace CoreBot.Models.Generate
{
    /// <summary>
    /// OBJETIVO: Geração do PDF de Ficha de Compensação
    /// AUTOR(ES): Felipe Falcão
    /// </summary>
    public class GeneratePdfCompensacao
    {

        public static void GenerateInvoice()
        {
            Document doc = new Document(PageSize.A4, 2F, 2F, 25F, 10F);

            string caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            FileStream file = new FileStream(caminho + "/Ficha de Compensação" + ".pdf", FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, file);

            WriteDocument(doc, writer);
        }

        public static byte[] GenerateInvoice2()
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Document document = new Document(PageSize.A4, 2F, 2F, 25F, 10F);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            WriteDocument(document, writer);

            byte[] bytes = memoryStream.ToArray();

            return bytes;
        }

        public static void WriteDocument(Document doc, PdfWriter writer)
        {
            doc.Open();
            Rectangle page = doc.PageSize;
            Font Titulo = FontFactory.GetFont("Verdana", 12F, Font.BOLD, BaseColor.BLACK);
            Font Subtitulo = FontFactory.GetFont("Verdana", 9F, Font.BOLD, BaseColor.BLACK);
            Font FontePadrao = FontFactory.GetFont("Verdana", 8F, Font.NORMAL, BaseColor.BLACK);
            Font FonteVia = FontFactory.GetFont("Verdana", 6F, Font.NORMAL, BaseColor.BLACK);
            Paragraph parag = new Paragraph(new Phrase("\n"));

            //string pathImageDetran = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "detran.jpeg");
            //string pathImageBanese = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "banese.jpg");

            string pathImageDetran = "https://www.detran.se.gov.br/portal/images/detran.jpeg";
            string pathImageBanese = "https://www.detran.se.gov.br/portal/images/banese.jpg";

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pathImageDetran);
            iTextSharp.text.Image imageBanese = iTextSharp.text.Image.GetInstance(pathImageBanese);

            doc.Add(Header());
            doc.Add(parag);
            doc.Add(tableAlerta(FontePadrao));
            doc.Add(parag);
            doc.Add(tableDUA(Titulo));
            doc.Add(tableDados(FontePadrao, page, image));
            doc.Add(tableDiscriminacao(FontePadrao, Subtitulo));
            doc.Add(parag);
            doc.Add(tableMultas(FontePadrao, Subtitulo));
            doc.Add(parag);
            doc.Add(tableObservacoes(FontePadrao, Subtitulo));
            doc.Add(parag);
            doc.Add(tablePendencias(FontePadrao, Subtitulo));
            doc.Add(parag);
            doc.Add(tableVia(writer, FonteVia, page, imageBanese, Subtitulo));
            doc.Close();
        }

        public static Paragraph Header()
        {
            DateTime thisDay = DateTime.Now;
            string dados = "DETRAN/SE - Portal de Serviços - Doc gerado eletronicamente em " + thisDay.ToString("dd/MM/yyyy") + " às " + thisDay.ToString("HH:mm:ss");
            Paragraph cabecalho = new Paragraph(dados, new Font(Font.NORMAL, 9));
            cabecalho.Alignment = Element.ALIGN_CENTER;
            return cabecalho;
        }

        public static PdfPTable tableAlerta(Font FontePadrao)
        {
            PdfPTable tableAlerta = new PdfPTable(1);
            PdfPCell cell = new PdfPCell(new Phrase("ALERTA: Ao emitir o boleto bancário é importante checar se os três primeiros dígitos da linha digitável do código de barras começam com \"047\"." +
                                                    "Se não, cuidado, seu computador pode estar infectado com vírus e o pagamento do boleto não será feito para o Detran/ SE.", FontePadrao));
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cell.Border = 1;
            tableAlerta.AddCell(cell);
            return tableAlerta;
        }

        public static PdfPTable tableDUA(Font Titulo)
        {
            PdfPTable tableDUA = new PdfPTable(1);
            PdfPCell cell0 = new PdfPCell(new Phrase("DOCUMENTO ÚNICO DE ARRECADAÇÃO - DUA/DETRAN - VALOR TOTAL", Titulo));
            cell0.HorizontalAlignment = 1;
            cell0.Border = 1;
            tableDUA.AddCell(cell0);
            return tableDUA;
        }

        public static PdfPTable tableDados(Font FontePadrao, Rectangle page, iTextSharp.text.Image image)
        {
            PdfPTable table1 = new PdfPTable(5);
            table1.TotalWidth = page.Width;

            PdfPCell cell1 = new PdfPCell();

            AddImageInCell(cell1, image, 50f, 50f, 1);
            cell1.HorizontalAlignment = 0;
            cell1.Border = 1;
            cell1.Padding = 4f;
            cell1.Rowspan = 4;
            table1.AddCell(cell1);

            float[] widths = new float[] { 70f, 170f, 150f, 100f, 100f };
            table1.SetWidths(widths);

            // LINHA 1
            PdfPCell cell2 = new PdfPCell(new Phrase("PLACA: \n" + FieldsGenerate.placa, FontePadrao));
            cell2.HorizontalAlignment = 0;
            cell2.Border = 1;
            table1.AddCell(cell2);

            PdfPCell cell7 = new PdfPCell(new Phrase("MARCA/MODELO:\n" + FieldsGenerate.marca, FontePadrao));
            cell7.HorizontalAlignment = 0;
            cell7.Border = 1;
            table1.AddCell(cell7);

            PdfPCell cell9 = new PdfPCell(new Phrase("COR:\n" + FieldsGenerate.cor, FontePadrao));
            cell9.HorizontalAlignment = 0;
            cell9.Border = 1;
            table1.AddCell(cell9);

            PdfPCell cell10 = new PdfPCell(new Phrase("EXERCÍCIO:\n" + FieldsGenerate.exercicio, FontePadrao));
            cell10.HorizontalAlignment = 0;
            cell10.Border = 1;
            table1.AddCell(cell10);

            // LINHA 2
            FieldsGenerate nome = new FieldsGenerate();
            PdfPCell cell5 = new PdfPCell(new Phrase("NOME: " + nome, FontePadrao));
            cell5.Colspan = 2;
            cell5.HorizontalAlignment = 0;
            cell5.Border = 1;
            table1.AddCell(cell5);

            PdfPCell cell6 = new PdfPCell(new Phrase("CHASSI: " + FieldsGenerate.chassi, FontePadrao));
            cell6.Colspan = 2;
            cell6.HorizontalAlignment = 0;
            cell6.Border = 1;
            table1.AddCell(cell6);

            // LINHA 3
            PdfPCell cell8 = new PdfPCell(new Phrase("TIPO:\n" + FieldsGenerate.tipo, FontePadrao));
            cell8.HorizontalAlignment = 0;
            cell8.Border = 1;
            table1.AddCell(cell8);

            PdfPCell cell11 = new PdfPCell(new Phrase("VALIDADE DESTE DOC:\n" + FieldsGenerate.vencimento, FontePadrao));
            cell11.HorizontalAlignment = 0;
            cell11.Border = 1;
            table1.AddCell(cell11);

            PdfPCell cell3 = new PdfPCell(new Phrase("NR. DOCUMENTO: \n" + FieldsGenerate.documento, FontePadrao));
            cell3.HorizontalAlignment = 0;
            cell3.Border = 1;
            cell3.Colspan = 2;
            table1.AddCell(cell3);

            // LINHA 4

            PdfPCell cell4 = new PdfPCell(new Phrase("DATA DO DOC:\n" + FieldsGenerate.processado, FontePadrao));
            cell4.HorizontalAlignment = 0;
            cell4.Border = 1;
            table1.AddCell(cell4);

            PdfPCell cell13 = new PdfPCell(new Phrase(FieldsGenerate.tituloVenc + " " + FieldsGenerate.dataVenc, FontePadrao));
            cell13.HorizontalAlignment = 0;
            cell13.Border = 1;
            cell13.Colspan = 3;
            table1.AddCell(cell13);

            return table1;
        }

        public static PdfPTable tableDiscriminacao(Font FontePadrao, Font Subtitulo)
        {
            PdfPTable tableDiscriminacao = new PdfPTable(2);
            float[] Discwidths = new float[] { 400f, 200f };
            tableDiscriminacao.SetWidths(Discwidths);

            PdfPCell cellDisc0 = new PdfPCell(new Phrase("DISCRIMINAÇÃO DOS DÉBITOS/CRÉDITOS", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableDiscriminacao.AddCell(cellDisc0);

            PdfPCell cellDisc1 = new PdfPCell(new Phrase("Taxa", Subtitulo));
            cellDisc1.HorizontalAlignment = 1;
            cellDisc1.Border = 1;
            tableDiscriminacao.AddCell(cellDisc1);

            PdfPCell cellDisc2 = new PdfPCell(new Phrase("Valor", Subtitulo));
            cellDisc2.HorizontalAlignment = 1;
            cellDisc2.Border = 1;
            tableDiscriminacao.AddCell(cellDisc2);

            List<string> taxa = new List<string>();
            taxa = FieldsGenerate.listaTaxas();

            List<string> preco = new List<string>();
            preco = FieldsGenerate.listaPreco();

            for (int i = 0; i < taxa.Count; i++)
            {
                PdfPCell cellDisc3 = new PdfPCell(new Phrase(taxa[i], FontePadrao));
                cellDisc3.HorizontalAlignment = 0;
                cellDisc3.Border = 0;
                tableDiscriminacao.AddCell(cellDisc3);

                PdfPCell cellDisc4 = new PdfPCell(new Phrase(preco[i], FontePadrao));
                cellDisc4.HorizontalAlignment = 2;
                cellDisc4.Border = 0;
                tableDiscriminacao.AddCell(cellDisc4);
            }

            return tableDiscriminacao;
        }

        public static PdfPTable tableMultas(Font FontePadrao, Font Subtitulo)
        {
            PdfPTable tableMulta = new PdfPTable(1);
            PdfPCell cellDisc0 = new PdfPCell(new Phrase("MULTAS", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableMulta.AddCell(cellDisc0);

            List<string> multas = new List<string>();
            multas = FieldsGenerate.multas();

            if (multas.Count > 0)
            {
                for (int i = 0; i < multas.Count; i++)
                {
                    PdfPCell cellDisc1 = new PdfPCell(new Phrase(multas[i], FontePadrao));
                    cellDisc1.HorizontalAlignment = 0;
                    cellDisc1.Border = 0;
                    tableMulta.AddCell(cellDisc1);
                }
            }
            else
            {
                PdfPCell cellDisc1 = new PdfPCell(new Phrase("\r\n", FontePadrao));
                cellDisc1.Border = 0;
                tableMulta.AddCell(cellDisc1);
            }

            return tableMulta;
        }

        public static PdfPTable tableObservacoes(Font FontePadrao, Font Subtitulo)
        {
            PdfPTable tableObs = new PdfPTable(1);
            PdfPCell cellDisc0 = new PdfPCell(new Phrase("OBSERVAÇÕES", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableObs.AddCell(cellDisc0);

            if (FieldsGenerate.mensagem1 != "")
            {
                PdfPCell cellDisc11 = new PdfPCell(new Phrase(FieldsGenerate.mensagem1, FontePadrao));
                cellDisc11.Border = 0;
                tableObs.AddCell(cellDisc11);
            }
            if (FieldsGenerate.mensagem2 != "")
            {
                PdfPCell cellDisc2 = new PdfPCell(new Phrase(FieldsGenerate.mensagem2, FontePadrao));
                cellDisc2.Border = 0;
                tableObs.AddCell(cellDisc2);
            }
            if (FieldsGenerate.mensagem3 != "")
            {
                PdfPCell cellDisc3 = new PdfPCell(new Phrase(FieldsGenerate.mensagem3, FontePadrao));
                cellDisc3.Border = 0;
                tableObs.AddCell(cellDisc3);
            }
            if (FieldsGenerate.mensagem4 != "")
            {
                PdfPCell cellDisc4 = new PdfPCell(new Phrase(FieldsGenerate.mensagem4, FontePadrao));
                cellDisc4.Border = 0;
                tableObs.AddCell(cellDisc4);
            }
            if (FieldsGenerate.mensagem5 != "")
            {
                PdfPCell cellDisc5 = new PdfPCell(new Phrase(FieldsGenerate.mensagem5, FontePadrao));
                cellDisc5.Border = 0;
                tableObs.AddCell(cellDisc5);
            }

            PdfPCell cellDisc1 = new PdfPCell(new Phrase("\r\n", FontePadrao));
            cellDisc1.Border = 0;
            tableObs.AddCell(cellDisc1);

            return tableObs;
        }

        public static PdfPTable tablePendencias(Font FontePadrao, Font Subtitulo)
        {
            PdfPTable tablePendencias = new PdfPTable(1);
            PdfPCell cellDisc0 = new PdfPCell(new Phrase("PENDÊNCIA(S) QUE IMPEDE(M) A EMISSÃO DO CRLV", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tablePendencias.AddCell(cellDisc0);

            List<string> pendencias = new List<string>();
            pendencias = FieldsGenerate.pendencias();

            if (pendencias.Count > 0)
            {
                for (int i = 0; i < pendencias.Count; i++)
                {
                    PdfPCell cellDisc1 = new PdfPCell(new Phrase(pendencias[i], FontePadrao));
                    cellDisc1.HorizontalAlignment = 0;
                    cellDisc1.Border = 0;
                    tablePendencias.AddCell(cellDisc1);
                }
            }
            else
            {
                PdfPCell cellDisc1 = new PdfPCell(new Phrase("\r\n", FontePadrao));
                cellDisc1.Border = 0;
                tablePendencias.AddCell(cellDisc1);
            }

            return tablePendencias;
        }

        public static PdfPTable tableVia(PdfWriter writer, Font FontePadrao, Rectangle page, iTextSharp.text.Image image, Font Subtitulo)
        {
            /// CABEÇALHO
            PdfPTable tableVia = new PdfPTable(6);
            tableVia.TotalWidth = page.Width;

            PdfPCell viaCellLine = new PdfPCell(new Phrase("---------------------------------------------------------------------------------------------------------------------"));
            viaCellLine.HorizontalAlignment = 1;
            viaCellLine.Border = 0;
            viaCellLine.Colspan = 6;
            tableVia.AddCell(viaCellLine);

            PdfPCell viaCellValor = new PdfPCell(new Phrase("VALOR", Subtitulo));
            viaCellValor.HorizontalAlignment = Element.ALIGN_CENTER;
            viaCellValor.Colspan = 6;
            tableVia.AddCell(viaCellValor);

            PdfPCell viaCell0 = new PdfPCell();
            AddImageInCell(viaCell0, image, 50f, 50f, 1);
            viaCell0.HorizontalAlignment = 0;
            viaCell0.Border = 0;
            viaCell0.Padding = 2f;
            viaCell0.Colspan = 1;
            tableVia.AddCell(viaCell0);

            PdfPCell viaCellConta = new PdfPCell(new Phrase("047-7", Subtitulo));
            viaCellConta.HorizontalAlignment = Element.ALIGN_CENTER;
            viaCellConta.Colspan = 1;
            tableVia.AddCell(viaCellConta);

            PdfPCell viaCellNum= new PdfPCell(new Phrase(FieldsGenerate.codBarras, Subtitulo));
            viaCellNum.HorizontalAlignment = Element.ALIGN_CENTER;
            viaCellNum.Border = 0;
            viaCellNum.Colspan = 4;
            tableVia.AddCell(viaCellNum);

            // LINHA 1 VIA
            PdfPCell viaCell1 = new PdfPCell(new Phrase("Local de Pagamento: \nEM QUALQUER BANCO. APÓS O VENCIMENTO DESCONSIDERE ESTE DOCUMENTO", FontePadrao));
            viaCell1.HorizontalAlignment = 0;
            viaCell1.Colspan = 5;
            tableVia.AddCell(viaCell1);

            PdfPCell viaCell2 = new PdfPCell(new Phrase("Vencimento: \n" + FieldsGenerate.vencimento, FontePadrao));
            viaCell2.HorizontalAlignment = 0;
            viaCell2.Colspan = 1;
            tableVia.AddCell(viaCell2);

            // LINHA 2 
            PdfPCell cell2 = new PdfPCell(new Phrase("Beneficiário: DEPARTAMENTO ESTADUAL DE TRÂNSITO DE SERGIPE - DETRAN/SE\n"
                                                    + FieldsGenerate.sacador
                                                    + " - Cnpj. 01.560.393 / 0001 - 50", FontePadrao));
            cell2.HorizontalAlignment = 0;
            cell2.Colspan = 4;
            tableVia.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Agên./Cód. Beneficiário.:\n " + FieldsGenerate.agencia, FontePadrao));
            cell3.HorizontalAlignment = 0;
            cell3.Colspan = 2;
            tableVia.AddCell(cell3);

            // LINHA 3 
            PdfPCell cell4 = new PdfPCell(new Phrase("Data do documento:\n" + FieldsGenerate.emissao, FontePadrao));
            cell4.HorizontalAlignment = 0;
            tableVia.AddCell(cell4);

            PdfPCell cell5 = new PdfPCell(new Phrase("Nº Documento:\n", FontePadrao));
            cell5.HorizontalAlignment = 0;
            tableVia.AddCell(cell5);

            PdfPCell cell6 = new PdfPCell(new Phrase("Espécie doc:\n RC", FontePadrao));
            cell6.HorizontalAlignment = 0;
            tableVia.AddCell(cell6);

            PdfPCell cell7 = new PdfPCell(new Phrase("Aceite:\n N", FontePadrao));
            cell7.HorizontalAlignment = 0;
            tableVia.AddCell(cell7);

            PdfPCell cell8 = new PdfPCell(new Phrase("Dt. Processamento:\n" + FieldsGenerate.processado, FontePadrao));
            cell8.HorizontalAlignment = 0;
            tableVia.AddCell(cell8);

            PdfPCell cell9 = new PdfPCell(new Phrase("Nosso Número:\n" + FieldsGenerate.nossonum, FontePadrao));
            cell9.HorizontalAlignment = 0;
            tableVia.AddCell(cell9);

            // LINHA 4 
            PdfPCell cell10 = new PdfPCell(new Phrase("Uso do Banco", FontePadrao));
            cell10.HorizontalAlignment = 0;
            tableVia.AddCell(cell10);

            PdfPCell cell11 = new PdfPCell(new Phrase("Carteira:\n CS", FontePadrao));
            cell11.HorizontalAlignment = 0;
            tableVia.AddCell(cell11);

            PdfPCell cell12 = new PdfPCell(new Phrase("Moeda:\n R$", FontePadrao));
            cell12.HorizontalAlignment = 0;
            tableVia.AddCell(cell12);

            PdfPCell cell13 = new PdfPCell(new Phrase("Quantidade\n -", FontePadrao));
            cell13.HorizontalAlignment = 0;
            tableVia.AddCell(cell13);

            PdfPCell cell14 = new PdfPCell(new Phrase("Valor:\n -", FontePadrao));
            cell14.HorizontalAlignment = 0;
            tableVia.AddCell(cell14);

            PdfPCell cell15 = new PdfPCell(new Phrase("(=) Valor do Documento: \nR$ " + FieldsGenerate.valorTotal, FontePadrao));
            cell15.HorizontalAlignment = 0;
            tableVia.AddCell(cell15);

            // LINHA 5
            PdfPCell cell16 = new PdfPCell(new Phrase("Instruções" +
                                                        "\nSR.CAIXA NÃO RECEBER APÓS O VENCIMENTO\r\n" +
                                                        FieldsGenerate.mensagem1 + " " + FieldsGenerate.mensagem2, FontePadrao));
            cell16.HorizontalAlignment = 0;
            cell16.Colspan = 4;
            cell16.Rowspan = 5;
            tableVia.AddCell(cell16);

            PdfPCell cell17 = new PdfPCell(new Phrase("(-) Descontos/Abatimento: ", FontePadrao));
            cell17.HorizontalAlignment = 0;
            cell17.Colspan = 2;
            tableVia.AddCell(cell17);

            PdfPCell cell18 = new PdfPCell(new Phrase("(-) Outras deduções", FontePadrao));
            cell18.HorizontalAlignment = 0;
            cell18.Colspan = 2;
            tableVia.AddCell(cell18);

            PdfPCell cell19 = new PdfPCell(new Phrase("(+) Mora/Multa", FontePadrao));
            cell19.HorizontalAlignment = 0;
            cell19.Colspan = 2;
            tableVia.AddCell(cell19);

            PdfPCell cell20= new PdfPCell(new Phrase("(+) Outros acréscimos", FontePadrao));
            cell20.HorizontalAlignment = 0;
            cell20.Colspan = 2;
            tableVia.AddCell(cell20);

            PdfPCell cell21 = new PdfPCell(new Phrase("(=) Valor cobrado: R$ " + FieldsGenerate.valorTotal, FontePadrao));
            cell21.HorizontalAlignment = 0;
            cell21.Colspan = 2;
            tableVia.AddCell(cell21);

            //LINHA 6
            FieldsGenerate nome = new FieldsGenerate();
            PdfPCell cell22 = new PdfPCell(new Phrase("Pagador: \n" + nome.nome, FontePadrao));
            cell22.HorizontalAlignment = 0;
            cell22.Colspan = 3;
            tableVia.AddCell(cell22);    

            PdfPCell cell24 = new PdfPCell(new Phrase("CPF/CNPJ: \n" + FieldsGenerate.cpf, FontePadrao));
            cell24.HorizontalAlignment = 0;
            cell24.Colspan = 3;
            tableVia.AddCell(cell24);

            //LINHA 7
            PdfPCell cell25 = new PdfPCell(new Phrase("Sacador/Avalista: " + FieldsGenerate.sacador, FontePadrao));
            cell25.HorizontalAlignment = 0;
            cell25.Colspan = 6;
            tableVia.AddCell(cell25);

            // LINHA 8 - CÓDIGO DE BARRAS
            PdfPCell viaCell5 = new PdfPCell();
            AddImageInCell(viaCell5, BarCode(writer), 350f, 350f, 1);
            viaCell5.HorizontalAlignment = 1;
            viaCell5.PaddingTop = 10;
            viaCell5.Colspan = 6;
            viaCell5.Border = 0;
            tableVia.AddCell(viaCell5);

            PdfPCell cellDisc8 = new PdfPCell(new Phrase("\r\n", FontePadrao));
            cellDisc8.Border = 0;
            cellDisc8.Colspan = 6;
            tableVia.AddCell(cellDisc8);

            // LINHA 9 VIA - NÚMERO CODIGO BARRA
            PdfPCell viaCell6 = new PdfPCell(new Phrase(FieldsGenerate.codBarras, FontePadrao));
            viaCell6.HorizontalAlignment = 1;
            viaCell6.Colspan = 6;
            viaCell6.Border = 0;
            tableVia.AddCell(viaCell6);

            PdfPCell cellDisc7  = new PdfPCell(new Phrase("\r\n", FontePadrao));
            cellDisc7.Border = 0;
            cellDisc7.Colspan = 6;
            tableVia.AddCell(cellDisc7);

            return tableVia;
        }

        private static void AddImageInCell(PdfPCell cell, iTextSharp.text.Image image, float fitWidth, float fitHight, int Alignment)
        {
            image.ScaleToFit(fitWidth, fitHight);
            image.Alignment = Alignment;
            cell.AddElement(image);
        }

        public static Image BarCode(PdfWriter writer)
        {
            PdfContentByte cb = writer.DirectContent;
            Barcode128 bc39 = new Barcode128();
            bc39.Code = FieldsGenerate.linhaCodBarras;
            bc39.Font = null;
            Image img = bc39.CreateImageWithBarcode(cb, null, null);
            return img;
        }
    }
}
