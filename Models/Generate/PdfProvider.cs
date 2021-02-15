using CoreBot.Fields;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Generate
{
    public class PdfProvider
    {
        /// <summary>
        /// Função responsável por Disponibilizar o PDF para download
        /// </summary>
        /// <param name="var">Array de bytes com dados necessários para criar o PDF</param>
        /// <returns>Attachment a ser chamado por "new List<Attachment>()"</returns>
        public static Attachment Disponibilizer(byte[] var)
        {
            var docData = Convert.ToBase64String(var);

            return new Attachment
            {
                Name = @"CRLVe",
                ContentType = "application/pdf",
                ContentUrl = $"data:application/pdf;base64,{docData}",
            };
        }

        /// <summary>
        /// Função responsável por converter o arquivo em array de bytes e disponibilizar o PDF para download
        /// </summary>
        /// <returns>Attachment a ser chamado por "new List<Attachment>()"</returns>
        private static Attachment Disponibilizer()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Assets/App", "icon_pdf_download.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"Assets\App\icon_pdf_download.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }

        /// <summary>
        /// Função responsável por Disponibilizar o PDF para download
        /// </summary>
        /// <param name="doc">String em base64 para disponibilização do PDF</param>
        /// <param name="name">Nome para o arquivo</param>
        /// <returns></returns>
        public static Attachment Disponibilizer(string doc, string name)
        {
            var docData = doc;
            var docName = name;

            return new Attachment
            {
                Name = docName + ".pdf",
                ContentType = "application/pdf",
                ContentUrl = $"data:application/pdf;base64,{docData}",
            };
        }
    }
}
