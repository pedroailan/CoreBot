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

namespace CoreBot.Models
{
    public class Generate
    {
        public static void GenerateInvoice(string Ano)
        {
            Document doc = new Document(PageSize.A4);//criando e estipulando o tipo da folha usada
            doc.SetMargins(5, 5, 1, 15);//estipulando o espaçamento das margens que queremos (E,D,C,B)
            doc.AddCreationDate(); //adicionando as configuracoes

            // Directory.GetCurrentDirectory()
            string caminho = @"C:\Users\fsfalcao\Downloads\" + "CONTRATO.pdf";

            FileStream arquivo = new FileStream(caminho, FileMode.Create); // Criação de arquivo
            PdfWriter writer = PdfWriter.GetInstance(doc, arquivo);

            WriteDocument(doc);
        }

        public static void WriteDocument(Document doc)
        {
            doc.Open();

            doc.Add(Header());
            doc.Add(new Paragraph("\r\n"));
            doc.Add(InfoAlert());

            doc.Close();
        }

        public static Paragraph Header()
        {
            string dados = "DETRAN/SE - Portal de Serviços";
            Paragraph cabecalho = new Paragraph(dados, new Font(Font.NORMAL, 9));
            cabecalho.Alignment = Element.ALIGN_LEFT;
            return cabecalho;
        }

        public static PdfPTable InfoAlert()
        {
            PdfPTable alerta = new PdfPTable(1);
            PdfPCell celula = new PdfPCell();
            celula.Phrase = new Phrase("ALERTA: Este Documento Único de Arrecadação – DUA somente pode ser quitado no Banco do Estado de Sergipe – BANESE, por isto não se faz necessário que o código de barra e sua respectiva linha digitavel inicie com o código do banco (047)");
            alerta.AddCell(celula);
            return alerta;
        }

        internal static void GenerateCRLVe(string anoExercicio)
        {
            throw new NotImplementedException();
        }
    }
}
