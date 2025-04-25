using Docnet.Core;
using Docnet.Core.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace PdfReaderApp.Services
{
    public class PdfToImageService
    {
        public List<string> ConverterPdfParaImagens(string caminhoPdf)
        {
            var imagensGeradas = new List<string>();

            using var docReader = DocLib.Instance.GetDocReader(caminhoPdf, new PageDimensions(1080, 1920));
            var pageCount = docReader.GetPageCount();

            for (int i = 0; i < pageCount; i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                var rawBytes = pageReader.GetImage();

                using var bmp = new Bitmap(pageReader.GetPageWidth(), pageReader.GetPageHeight(), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);
                bmp.UnlockBits(bmpData);

                var tempPath = Path.Combine(Path.GetTempPath(), $"pagina_{i}_{Guid.NewGuid()}.png");
                bmp.Save(tempPath, ImageFormat.Png);

                imagensGeradas.Add(tempPath);
            }

            return imagensGeradas;
        }
    }
}
