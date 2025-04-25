using Tesseract;

namespace PdfReaderApp.Services
{
    public class OcrService
    {
        private readonly string _tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

        public string LerTextoImagem(string caminhoImagem, string idioma = "por")
        {
            using var engine = new TesseractEngine(_tessDataPath, idioma, EngineMode.Default);
            using var img = Pix.LoadFromFile(caminhoImagem);
            using var page = engine.Process(img);

            return page.GetText();
        }
    }

}
