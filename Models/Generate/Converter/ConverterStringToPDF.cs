using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Generate.Converter
{
    public static class ConverterStringToPDF
    {

        public static void converter(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);

            string caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            System.IO.FileStream stream = new FileStream(caminho + "/file.pdf", FileMode.CreateNew);
            System.IO.BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        }
    }
}
