using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO; // Entrada e saída de arquivos
using iTextSharp; // Biblioteca ITextSharp
using iTextSharp.text; // Extensão 1 - Text
using iTextSharp.text.pdf; // Extensão 2 - PDF
using iTextSharp.text.html.simpleparser; //
using CoreBot.Models.Generate;

namespace CoreBot.Models
{
    /// <summary>
    /// OBJETIVO: Geração do PDF do Documento de Arrecadação (DUA)
    /// AUTOR(ES): Felipe Falcão
    /// </summary>
    public class GeneratePdfDUA
    {
        public static void GenerateInvoice()
        {
            Document doc = new Document(PageSize.A4, 2F, 2F, 25F, 10F);

            string caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            FileStream file = new FileStream(caminho + "/DUA - " + FieldsGenerate.nome + ".pdf", FileMode.Create);

            PdfWriter writer = PdfWriter.GetInstance(doc, file);

            WriteDocument(doc, writer);
        }

        public static byte[] GenerateInvoice2()
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Document document = new Document(PageSize.A4, 2F, 2F, 25F, 10F);            
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            WriteDocument(document, writer);
            //document.Open();
            //document.Add(new Paragraph(new Phrase("TESTE")));
            //document.Close();

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
            Paragraph parag = new Paragraph(new Phrase("\n"));

            //string pathImageDetran = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "detran.jpeg");
            string pathImageDetran = "https://www.detran.se.gov.br/portal/images/detran.jpeg";
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pathImageDetran);

            doc.Add(Header());
            //doc.Add(new Paragraph(new Phrase(Environment.CurrentDirectory)));
            //doc.Add(new Paragraph(new Phrase(Directory.GetCurrentDirectory())));
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
            doc.Add(tableInfoPagamento(FontePadrao, Subtitulo));
            doc.Add(tableVia(writer, FontePadrao, page, image));
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
            PdfPCell cell = new PdfPCell(new Phrase("ALERTA: Este Documento Único de Arrecadação – DUA somente pode ser quitado no Banco do Estado de Sergipe – BANESE, por isto não se faz necessário que o código de barra e sua respectiva linha digitavel inicie com o código do banco(047).", FontePadrao));
            cell.HorizontalAlignment = 1;
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
            PdfPCell cell2 = new PdfPCell(new Phrase("PLACA: " + FieldsGenerate.placa, FontePadrao));
            cell2.HorizontalAlignment = 0;
            cell2.Border = 1;
            table1.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("DOCUMENTO: " + FieldsGenerate.documento, FontePadrao));
            cell3.HorizontalAlignment = 0;
            cell3.Border = 1;
            table1.AddCell(cell3);

            PdfPCell cell4 = new PdfPCell(new Phrase("FOLHA: 01", FontePadrao));
            cell4.HorizontalAlignment = 0;
            cell4.Border = 1;
            cell4.Colspan = 2;
            table1.AddCell(cell4);

            // LINHA 2
            PdfPCell cell5 = new PdfPCell(new Phrase("NOME: " + FieldsGenerate.nome, FontePadrao));
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
            PdfPCell cell7 = new PdfPCell(new Phrase("MARCA/MODELO:\n" + FieldsGenerate.marca, FontePadrao));
            cell7.HorizontalAlignment = 0;
            cell7.Border = 1;
            table1.AddCell(cell7);

            PdfPCell cell8 = new PdfPCell(new Phrase("TIPO:\n" + FieldsGenerate.tipo, FontePadrao));
            cell8.HorizontalAlignment = 0;
            cell8.Border = 1;
            table1.AddCell(cell8);

            PdfPCell cell9 = new PdfPCell(new Phrase("COR:\n" + FieldsGenerate.cor, FontePadrao));
            cell9.HorizontalAlignment = 0;
            cell9.Border = 1;
            table1.AddCell(cell9);

            PdfPCell cell10 = new PdfPCell(new Phrase("EXERCÍCIO:\n" + FieldsGenerate.exercicio, FontePadrao));
            cell10.HorizontalAlignment = 0;
            cell10.Border = 1;
            table1.AddCell(cell10);

            // LINHA 4
            PdfPCell cell11 = new PdfPCell(new Phrase("PROCESSADO:\n" + FieldsGenerate.processado, FontePadrao));
            cell11.HorizontalAlignment = 0;
            cell11.Border = 1;
            table1.AddCell(cell11);

            PdfPCell cell12 = new PdfPCell(new Phrase("EMISSÃO:\n" + FieldsGenerate.emissao, FontePadrao));
            cell12.HorizontalAlignment = 0;
            cell12.Border = 1;
            table1.AddCell(cell12);

            PdfPCell cell13 = new PdfPCell(new Phrase(FieldsGenerate.tituloVenc + ": " + FieldsGenerate.dataVenc, FontePadrao));
            cell13.HorizontalAlignment = 0;
            cell13.Border = 1;
            cell13.Colspan = 2;
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

            List<string> obs = new List<string>();
            obs = FieldsGenerate.obs();

            if (obs.Count > 0)
            {
                for (int i = 0; i < obs.Count; i++)
                {
                    PdfPCell cellDisc1 = new PdfPCell(new Phrase(obs[i], FontePadrao));
                    cellDisc1.HorizontalAlignment = 0;
                    cellDisc1.Border = 0;
                    tableObs.AddCell(cellDisc1);

                }
            }
            else
            {
                PdfPCell cellDisc1 = new PdfPCell(new Phrase("\r\n", FontePadrao));
                cellDisc1.Border = 0;
                tableObs.AddCell(cellDisc1);
            }

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

        public static PdfPTable tableInfoPagamento(Font FontePadrao, Font Subtitulo)
        {
            PdfPTable tablePagamento = new PdfPTable(1);
            PdfPCell cellDisc0 = new PdfPCell(new Phrase("PAGÁVEL APENAS NO BANESE", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tablePagamento.AddCell(cellDisc0);

            PdfPCell cellInfo1 = new PdfPCell(new Phrase("- Somente após a quitação de todo o valor correspondente ao total do licenciamento o CRLV será impresso.", FontePadrao));
            cellInfo1.HorizontalAlignment = 0;
            cellInfo1.Border = 0;
            cellInfo1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablePagamento.AddCell(cellInfo1);

            PdfPCell cellInfo2 = new PdfPCell(new Phrase("- O pagamento em cheque deverá ser do proprietário e de igual valor ao constante neste documento, e somente após a compensação bancária será emitido o CRLV.", FontePadrao));
            cellInfo2.HorizontalAlignment = 0;
            cellInfo2.Border = 0;
            cellInfo2.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablePagamento.AddCell(cellInfo2);

            PdfPCell cellInfo3 = new PdfPCell(new Phrase("- Consulte nosso site - www.detran.se.gov.br - as informações do seu veículo estão atualizadas e disponiveis na internet.", FontePadrao));
            cellInfo3.HorizontalAlignment = 0;
            cellInfo3.Border = 0;
            cellInfo3.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablePagamento.AddCell(cellInfo3);

            PdfPCell cellInfo4 = new PdfPCell(new Phrase("- Ouvidoria geral: você tem mais um canal de comunicação com o DETRAN - fone:3226- 2072 - email: ouvidoria@detran.se.gov.br", FontePadrao));
            cellInfo4.HorizontalAlignment = 0;
            cellInfo4.Border = 0;
            cellInfo4.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            tablePagamento.AddCell(cellInfo4);

            return tablePagamento;
        }

        public static PdfPTable tableVia(PdfWriter writer, Font FontePadrao, Rectangle page, iTextSharp.text.Image image)
        {

            PdfPTable tableVia = new PdfPTable(5);
            tableVia.TotalWidth = page.Width;

            PdfPCell viaCellLine = new PdfPCell(new Phrase("---------------------------------------------------------------------------------------------------------------------"));
            viaCellLine.HorizontalAlignment = 1;
            viaCellLine.Border = 0;
            viaCellLine.Colspan = 5;
            tableVia.AddCell(viaCellLine);

            PdfPCell viaCell0 = new PdfPCell();
            AddImageInCell(viaCell0, image, 50f, 50f, 1);
            viaCell0.HorizontalAlignment = 0;
            viaCell0.Border = 0;
            viaCell0.Padding = 2f;
            viaCell0.Rowspan = 3;
            tableVia.AddCell(viaCell0);

            // LINHA 1 VIA
            PdfPCell viaCell1 = new PdfPCell(new Phrase("PLACA: " + FieldsGenerate.placa, FontePadrao));
            viaCell1.HorizontalAlignment = 0;
            tableVia.AddCell(viaCell1);

            PdfPCell viaCell2 = new PdfPCell(new Phrase("VAL DUA: " + FieldsGenerate.vencimento, FontePadrao));
            viaCell2.HorizontalAlignment = 0;
            tableVia.AddCell(viaCell2);

            PdfPCell viaCell3 = new PdfPCell(new Phrase("DOC: " + FieldsGenerate.documento, FontePadrao));
            viaCell3.HorizontalAlignment = 0;
            tableVia.AddCell(viaCell3);

            PdfPCell viaCell4 = new PdfPCell(new Phrase("VALOR: R$ " + FieldsGenerate.valorTotal, FontePadrao));
            viaCell4.HorizontalAlignment = 0;
            tableVia.AddCell(viaCell4);

            // LINHA 2 - CÓDIGO DE BARRAS
            PdfPCell viaCell5 = new PdfPCell();
            AddImageInCell(viaCell5, BarCode(writer), 350f, 350f, 1);
            viaCell5.HorizontalAlignment = 1;
            viaCell5.PaddingTop = 10;
            viaCell5.PaddingBottom = 10;
            viaCell5.Colspan = 4;
            viaCell5.Border = 0;
            tableVia.AddCell(viaCell5);

            // LINHA 3 VIA - NÚMERO CODIGO BARRA
            PdfPCell viaCell6 = new PdfPCell(new Phrase(FieldsGenerate.linhaDig, FontePadrao));
            viaCell6.HorizontalAlignment = 1;
            viaCell6.Colspan = 4;
            viaCell6.Border = 0;
            tableVia.AddCell(viaCell6);

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
            bc39.Code = FieldsGenerate.linhaDig.Replace(" ", "");
            bc39.Font = null;
            Image img = bc39.CreateImageWithBarcode(cb, null, null);
            return img;
        }
    }
}
